/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public record class ListQueryResult<TRecord> : IDataResult
{
    public IEnumerable<TRecord> Items { get; init; } = Enumerable.Empty<TRecord>();
    public bool Successful { get; init; }
    public Exception? Exception { get; init; } = new Exception("This Result instance has not been Initialized.");
    public int TotalCount { get; init; }

    public string? Message => this.Exception?.Message ?? null;

    public ListQueryResult() { }

    public TResult Map<TResult>(Func<IEnumerable<TRecord>, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return Successful ? onSuccess(this.Items!) : onFailure(this.Message!);
    }

    public TResult Map<TResult>(Func<IEnumerable<TRecord>, TResult> onSuccess, Func<Exception, TResult> onFailure)
    {
        return Successful ? onSuccess(this.Items!) : onFailure(this.Exception!);
    }


    public static ListQueryResult<TRecord> Success(IEnumerable<TRecord> Items, int totalCount)
    {
        return new ListQueryResult<TRecord>
        {
            Successful = true,
            Items = Items,
            TotalCount = totalCount,
            Exception = null
        };
    }

    public static ListQueryResult<TRecord> Failure(string message)
    {
        return new ListQueryResult<TRecord> { Exception = new ListQueryException(message) };
    }

    public static ListQueryResult<TRecord> Failure(Exception exception)
    {
        return new ListQueryResult<TRecord> { Exception = exception };
    }
}
