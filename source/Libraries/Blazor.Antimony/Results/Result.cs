/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Diagnostics.CodeAnalysis;

namespace Blazr.Antimony.Core;

public readonly record struct Result<T>
{
    // Hidden
    private T? _value { get; init; }
    private Exception? _error { get; init; }

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    private Result(Exception error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    public void Map(Action<T> onSuccess, Action<Exception> onFail)
    {
        if (this.IsSuccess)
            onSuccess(_value);
        else
            onFail(_error);
    }

    public void MapSuccess(Action<T> onSuccess)
    {
        if (this.IsFailure)
            throw new InvalidOperationException("You can't call MapSuccess on an error result.");

        onSuccess(_value!);
    }

    public void MapFailure(Action<Exception> onFail)
    {
        if (this.IsSuccess)
            throw new InvalidOperationException("You can't call MapFailure on a success.");

        onFail(_error);
    }

    public bool HasSucceeded([NotNullWhen(true)] out T? item)
    {
        if (this.IsSuccess)
            item = _value;
        else
            item = default;

        return this.IsSuccess;
    }

    public bool HasFailed([NotNullWhen(false)] out T? item)
    {
        if (this.IsSuccess)
            item = _value;
        else
            item = default;

        return this.IsFailure;
    }

    public Result<TOut> Convert<TOut>(TOut value)
    {
        if (this.IsFailure)
            throw new InvalidOperationException("You can't provide a value if the opertation has failed.");

        return new Result<TOut>() { _error = this._error, _value = value  };
    }
    
    public Result<TOut> Convert<TOut>()
    {
        if (this.IsSuccess)
            throw new InvalidOperationException("You must provide a value if the opertation has succeeded.");

        return Result<TOut>.Fail(this._error);
    }

    public static Result<T> Success(T value) => new(value);
public static Result<T> Fail(Exception error) => new(error);

public static implicit operator Result<T>(T value) => Success(value);
}
