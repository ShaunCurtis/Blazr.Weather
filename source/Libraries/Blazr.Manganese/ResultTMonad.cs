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

    public Result<TOut> Map<TOut>(Func<T, Result<TOut>> success, Func<Exception, Result<TOut>>? failure = null)
    {
        if (_exception is null)
            return success(_value!);

        if (_exception is not null && failure != null)
            return failure(_exception!);

        return Result<TOut>.Failure(_exception!);
    }

    public Result<U> Map<U>(Func<T, U> mapping)
    {
        if (_exception is not null)
            return Result<U>.Failure(_exception!);

        try
        {
            return Result<U>.Create(mapping(_value!));
        }
        catch (Exception ex)
        {
            return Result<U>.Failure(ex);
        }
    }

    public Result Map(Func<T, Result>? mapping = null)
    {
        if (_value is not null && mapping != null)
            return mapping(_value!);

        if (_value is not null)
            return Result.Success();

        return Result.Failure(_exception ?? _defaultException);
    }

    public async Task<Result<TOut>> MapAsync<TOut>(Func<T, Task<Result<TOut>>> success, Func<Exception, Task<Result<TOut>>>? failure = null)
    {
        if (_exception is null)
            return await success(_value!);

        if(_exception is not null && failure != null)
            return await failure(_exception!);

        return Result<TOut>.Failure(_exception ?? _defaultException);
    }

    public void Output(Action<T>? success = null, Action<Exception>? failure = null)
    {
        if (_exception is null && success != null)
            success(_value!);

        if (_exception is not null && failure != null)
            failure(_exception!);
    }
}
