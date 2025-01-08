
using System.Diagnostics.CodeAnalysis;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct Result<T>
{
    // Hidden
    private readonly T? _value;
    private readonly Exception? _error;

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
    private bool IsSuccess { get; }

    public Result<TReturn> Map<TReturn>(
        Func<T, TReturn> onSuccess,
        Func<Exception, Exception> onFailure)
    {
        return this.IsSuccess
            ? onSuccess(_value)
            : Result<TReturn>.Fail(onFailure(_error));
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(Exception error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
}
