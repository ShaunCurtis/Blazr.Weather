/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct DataResult : IDataResult
{
    public bool Successful { get; init; } = true;
    public string? Message { get; init; }

    public DataResult() { }

    public static DataResult Success(string? message = null)
    {
        return new DataResult { Successful = true, Message = message };
    }

    public static DataResult Failure(string message)
    {
        return new DataResult { Message = message };
    }
}

public readonly record struct DataResult<TData> : IDataResult
{
    public TData? Item { get; init; }
    public bool Successful { get; init; }
    public string? Message { get; init; }

    public DataResult() { }

    public TResult Map<TResult>(Func<TData, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return Successful ? onSuccess(this.Item!) : onFailure(this.Message!);
    }

    public static DataResult<TData> Success(TData Item, string? message = null)
    {
        return new DataResult<TData> { Successful = true, Item = Item, Message = message };
    }

    public static DataResult<TData> Failure(string message)
    {
        return new DataResult<TData> { Message = message };
    }
}

