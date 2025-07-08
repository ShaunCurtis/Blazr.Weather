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
    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");

    private Result(T? value) 
        => _value = value;

    private Result(Exception? exception) 
        => _exception = exception ?? _defaultException;

    private Result() 
        => _exception = _defaultException;

    public static Result<T> Create(T? value) => 
        value is null
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

    public Result<TOut> MapOut<TOut>(Func<T, Result<TOut>> success, Func<Exception, Result<TOut>>? failure = null)
    {
        if (_exception is null)
            return success(_value!);

        if (_exception is not null && failure != null)
            return failure(_exception!);

        return Result<TOut>.Failure(_exception!);
    }

    public Result<T> Map(Func<T, Result<T>>? success = null, Func<Exception, Result<T>>? failure = null)
    {
        if (_exception is null && success != null)
            return success(_value!);

        if (_exception is not null && failure != null)
            return failure(_exception!);

        return this;
    }

    public Result<U> Map<U>(Func<T, U> func)
    {
        if (_exception is not null)
            return Result<U>.Failure(_exception!);

        try
        {
            return Result<U>.Create(func(_value!));
        }
        catch (Exception ex)
        {
            return Result<U>.Failure(ex);
        }
    }

    public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<Result<TOut>>> success, Func<Exception, Task<Result<TOut>>>? failure = null)
    {
        if (_exception is null)
            return await success(_value!);

        if(_exception is not null && failure != null)
            return await failure(_exception!);

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

    public void Output(Action<T>? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null && success != null)
            success(_value!);

        if (_exception is not null && failure != null)
            failure(_exception!);
    }
}
