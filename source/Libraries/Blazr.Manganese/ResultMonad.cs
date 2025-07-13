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

    public Result MapToResult(Func<Result> mapping)
        => _exception is null
            ? mapping()
            : this;

    public Result<T> MapToResult<T>(Func<Result<T>> success, Func<Exception, Result<T>>? failure = null)
    {
        if (_exception is null)
            return success();

        if (_exception is not null && failure != null)
            return failure(_exception);

        return Result<T>.Failure(_exception!);
    }

    public Result MapToResult(bool test, Func<Result> isTrue, Func<Result> isFalse)
    {
        if (_exception is not null)
            return this;

        if (test)
            return isTrue();

        return isFalse();
    }

    public async Task<Result> MapToResultAsync(bool test, Func<Task<Result>> isTrue, Func<Task<Result>> isFalse)
    {
        if (_exception is not null)
            return Result.Failure(_exception!);

        return test ? await isTrue() : await isFalse();
    }

    public Result SideEffect(Action? success = null, Action<Exception>? failure = null)
    {
        Output(success, failure);

        return this;
    }

    public void Output(Action? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null && success != null)
            success();

        if (_exception is not null && failure != null)
            failure(_exception!);
    }
}
