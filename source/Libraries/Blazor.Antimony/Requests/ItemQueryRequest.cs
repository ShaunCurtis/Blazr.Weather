/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct ItemQueryRequest<TRecord>
{
    public Expression<Func<TRecord, bool>> FindExpression { get; private init; }
    public CancellationToken Cancellation { get; private init; }

    public ItemQueryRequest(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
    {
        this.FindExpression = expression;
        this.Cancellation = cancellation ?? new(); 
    }
    public static ItemQueryRequest<TRecord> Create(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
        => new ItemQueryRequest<TRecord>(expression, cancellation ?? new());
}
