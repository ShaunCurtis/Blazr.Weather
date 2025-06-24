/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;

    private Result(T? value)
        => _value = value;

    private Result(Exception? exception)
        => _exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

    private Result()
        => _exception = new ResultException("An error occurred. No specific exception provided.");

    public static Result<T> Return(T value) => new(value);
    public static Result<T> Return(Exception exception) => new(exception);
    public static Result<T> ReturnException(string message) => new(new ResultException(message));

    public void Match(Action<T> success, Action<Exception> failure)
    {
        if (_exception is null)
            success(_value!);
        else
            failure(_exception!);
    }

    public void MatchSuccess(Action<T> success)
    {
        if (_exception is null)
            success(_value!);
    }

    public void MatchFailure(Action<Exception> failure)
    {
        if (_exception is not null)
            failure(_exception!);
    }

    public TOut MapOut<TOut>(Func<T, TOut> success, Func<TOut> failure)
        => _exception is null
            ? success(_value!)
            : failure();

    public Result<U> Bind<U>(Func<T, Result<U>> func)
    {
        return _exception is null
            ? func(_value!)
            : Result<U>.Return(_exception!);
    }

    public Result<U> Bind<U>(Func<T, U> func)
    {
        if (_exception is not null)
            return Result<U>.Return(_exception!);

        try
        {
            return Result<U>.Return(func(_value!));
        }
        catch (Exception ex)
        {
            return Result<U>.Return(ex);
        }
    }

    public Result<T> Bind(Func<T, T> func)
    {
        if (_exception is not null)
            return this;

        try
        {
            return Result<T>.Return(func(_value!));
        }
        catch (Exception ex)
        {
            return Result<T>.Return(ex);
        }
    }

    public Result Bind(Action<T> action)
    {
        if (_exception is not null)
            return Result.Return(_exception!);

        try
        {
            action(_value!);
            return Result.Return();
        }
        catch (Exception ex)
        {
            return Result.Return(ex);
        }
    }

    public Result Bind(Func<T, Result> func)
        => _exception is null
            ? func(_value!)
            : Result.Return(_exception);

    public Result Map()
        => _exception is null
            ? Result.Return()
            : Result.Return(_exception!);

    public Result<T> Map(Func<T, Result<T>> success, Func<Exception, Result<T>> failure)
        => _exception is null
            ? success(_value!)
            : failure(_exception!);

    public Result<U> Map<U>(Func<T, Result<U>> success, Func<Exception, Result<U>> failure)
        => _exception is null
            ? success(_value!)
            : failure(_exception!);

    public Result Map(Func<T, Result> success, Func<Exception, Result> failure)
        => _exception is null
            ? success(_value!)
            : failure(_exception!);

    public Result<T> MapSuccess(Func<T, Result<T>> success)
        => _exception is null
            ? success(_value!)
            : this;

    public Result MapSuccess(Func<T, Result> success)
        => _exception is null
            ? success(_value!)
            : Result.Return(_exception);

    public Result<U> MapSuccess<U>(Func<T, Result<U>> success)
        => _exception is null
            ? success(_value!)
            : Result<U>.Return(_exception);

    public Result<T> MapFailure(Func<T, Result<T>> failure)
        => _exception is null
            ? this
            : failure(_value!);
}

/// <summary>
///  Object implementing a functional approach to result management
/// All constructors are private
/// Create instances through the provided static methods
/// </summary>
public record Result
{
    private readonly Exception? _exception;

    private Result(Exception? exception)
    {
        _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");
    }

    private Result() { }

    public static Result Return() => new();
    public static Result Return(Exception? exception) => new(exception);
    public static Result ReturnException(string message) => new(new ResultException(message));

    /// <summary>
    /// Binds a function to the Result instance.
    /// </summary>
    /// <param name="bind"></param>
    /// <returns></returns>
    public static Result Bind(Func<Result> bind) => bind();

    /// <summary>
    /// Return based on the instance exception state:
    ///  - A Result<T> containing the exception if in exception state
    ///  - The Result<T> provided by the delegate
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="func"></param>
    /// <returns></returns>
    public Result<T> Bind<T>(Func<Result<T>> func)
        => _exception is null
            ? func()
            : Result<T>.Return(_exception!);

    /// <summary>
    /// Maps a success or failure to the provided actions
    /// The underlying Result is unchanged
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    public void Match(Action success, Action<Exception> failure)
    {

        if (_exception is not null)
            failure(_exception!);
        else
            success();
    }
    /// <summary>
    /// Maps a success or failure to the provided functions
    /// The Result is provided by the delegates
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public Result Map(Func<Result> success, Func<Exception, Result> failure)
        => _exception is null
            ? success()
            : failure(_exception!);

    /// <summary>
    /// Maps a success to the provided function
    /// The success Result is provided by the delegates
    /// The failure Result is original Result
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public Result MapSuccess(Func<Result> success)
        => _exception is null
            ? success()
            : this;

    /// <summary>
    /// Maps a success to the provided function
    /// The success Result is provided by the delegates
    /// The failure Result is original Result
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="success"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Result MapSuccess<T>(Func<T, Result> success, T value)
        => _exception is null
            ? success(value)
            : this;

    /// Maps a success to the provided function
    /// The success Result is provided by the delegates
    /// The failure Result is original Result
    public Result<T> MapSuccess<T>(Func<Result<T>> success)
        => _exception is null
            ? success()
            : Result<T>.Return(_exception!);

    /// <summary>
    /// Maps a failure to the provided function
    /// The failure Result is provided by the delegates
    /// The success Result is original Result
    /// </summary>
    /// <param name="failure"></param>
    /// <returns></returns>
    public Result MapFailure(Func<Exception, Result> failure)
        => _exception is null
            ? this
            : failure(_exception!);

    /// <summary>
    /// Maps a failure to the provided Action
    /// There is no result
    /// </summary>
    /// <param name="failure"></param>
    public void MapFailure(Action<Exception> failure)
    {
        if (_exception is not null)
            failure(_exception!);
    }
}
