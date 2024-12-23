/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public sealed record ItemQueryResult<TRecord> : IDataResult
{
    public TRecord? Item { get; private init;}
    public bool Successful { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;

    public ItemQueryResult() { }

    public TResult Map<TResult>(Func<TRecord, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return Successful ? onSuccess(this.Item!) : onFailure(this.Message!);
    }

    public TResult Map<TResult>(Func<TRecord, TResult> onSuccess, Func<Exception, TResult> onFailure)
    {
        return Successful ? onSuccess(this.Item!) : onFailure(this.Exception!);
    }

    public static ItemQueryResult<TRecord> Success(TRecord Item)
    {
        return new ItemQueryResult<TRecord> { Successful = true, Item = Item, Exception = null };
    }

    public static ItemQueryResult<TRecord> Failure(string message)
    {
        return new ItemQueryResult<TRecord> { Exception = new ItemQueryException(message) };
    }

    public static ItemQueryResult<TRecord> Failure(Exception exception)
    {
        return new ItemQueryResult<TRecord> { Exception = exception };
    }
}
