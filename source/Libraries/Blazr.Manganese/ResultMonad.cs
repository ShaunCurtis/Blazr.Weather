/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

/// <summary>
///  Object implementing a functional approach to result management
/// All constructors are private
/// Create instances through the provided static methods
/// </summary>

public partial record Result
{
    private readonly Exception? _exception;

    private Result(Exception? exception)
        => _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    public Result AndThen(Func<Result> success)
        => _exception is null
        ? success()
        : this;

    public Result<TOut> AndThen<TOut>(Func<Result<TOut>> success)
        => _exception is null
        ? success()
        : Result<TOut>.Failure(_exception);
}


public partial record Result
{
    public static Result Return() => new();
    public static Result Return(Exception? exception) => new(exception);
    public static Result ReturnException(string message) => new(new ResultException(message));

    /// <summary>
    /// Return based on the instance exception state:
    ///  - The current result if exception
    ///  - A Result provided by the delegate
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public Result Bind(Func<Result> success)
        => _exception is null
            ? success()
            : this;

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
    /// Runs the provided action on Success
    /// The underlying Result is unchanged
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    public void MatchSuccess(Action success)
    {
        if (_exception is null)
            success();
    }

    /// <summary>
    /// Runs the provided action on Failure
    /// The underlying Result is unchanged
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    public void MatchFailure(Action<Exception> failure)
    {
        if (_exception is not null)
            failure(_exception!);
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
    /// Maps a success or failure to the provided functions
    /// The Result is provided by the delegates
    /// </summary>
    /// <param name="success"></param>
    /// <param name="failure"></param>
    /// <returns></returns>
    public Result Map(Action success, Action<Exception> failure)
    {
        if (_exception is null)
            success();
        else
            failure(_exception!);

        return this;
    }

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
    /// Maps a success to the provided Action
    /// The original Result is passed through regardless of the state
    /// </summary>
    /// <param name="success"></param>
    /// <returns></returns>
    public Result MapSuccess(Action success)
    {
        if (_exception is null)
            success();

        return this;
    }

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
