/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public partial record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;

    private Result(T? value)
        => _value = value;

    private Result(Exception? exception)
        => _exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

    private Result()
        => _exception = new ResultException("An error occurred. No specific exception provided.");


    public static Result<T> Create(T? value) => value is null
    ? new(new ResultException("T was null."))
    : new(value);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));

    public Result<T> SideEffect(Action<T> success, Action<Exception> failure)
    {
        if (_exception is null)
            success(_value!);
        else
            failure(_exception!);

        return this;
    }

    public Result<T> SideEffect(Action<T> success)
    {
        if (_exception is null)
            success(_value!);

        return this;
    }

    public Result<T> SideEffect(Action<Exception> failure)
    {
        if (_exception is not null)
            failure(_exception!);

        return this;
    }

    public Result<TOut> Map<TOut>(Func<T, Result<TOut>> success, Func<Exception, Result<TOut>> failure)
    {
        if (_exception is null)
            return success(_value!);

        return failure(_exception!);
    }

    public async Task<Result<TOut>> Map<TOut>(Func<T, Task<Result<TOut>>> success, Func<Exception, Task<Result<TOut>>> failure)
    {
        if (_exception is null)
            return await success(_value!);

        return await failure(_exception!);
    }

    public async Task<Result<TOut>> MapSuccess<TOut>(Func<T, Task<Result<TOut>>> success)
    {
        if (_exception is null)
            return await success(_value!);

        return Result<TOut>.Failure(_exception!);
    }

    public Result MapToResult()
    {
        if (_exception is null)
            return Result.Success();

        return Result.Failure(_exception!);
    }

    public Result<T> MapOnSuccess<TOut>(Func<T, Result<TOut>> success)
    {
        if (_exception is null)
            success(_value!);

        return this;
    }

    public Result<T> MapOnFailure(Func<Exception, Result<T>> failure)
    {
        if (_exception is not null)
            return failure(_exception!);

        return this;
    }
}

public partial record Result<T>
{
    /// <summary>
    /// Returns a success/failure Result<T> with the provided value based on the null state of the value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<T> Return(T? value) => value is null
        ? new(new ResultException("T was null."))
        : new(value);

    /// <summary>
    /// Returns a failure Result<T> with the provided exception.
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static Result<T> Return(Exception exception) => new(exception);

    /// <summary>
    /// Returns a failure result with the message wrapped in a ResultException
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static Result<T> ReturnException(string message) => new(new ResultException(message));

    /// <summary>
    /// Provides the way to unwrap a Result based on success or failure 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    public Result<T> Match(Action<T> success, Action<Exception> failure)
    {
        if (_exception is null)
            success(_value!);
        else
            failure(_exception!);

        return this;
    }

    public Result<T> MatchSuccess(Action<T> success)
    {
        if (_exception is null)
            success(_value!);

        return this;
    }

    public Result<T> MatchFailure(Action<Exception> failure)
    {
        if (_exception is not null)
            failure(_exception!);

        return this;
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

    public Result<U> Map<U>(Func<T, U> func)
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

    public Result Map(Action<T> action)
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
    public Result Map(Func<T, Result> success, Func<Exception, Result> failure)
        => _exception is null
            ? success(_value!)
            : failure(_exception!);

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
