/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Antimony.Core;

public interface IListRequestHandler
{
    public ValueTask<Result<ListResult<TRecord>>> ExecuteAsync<TRecord>(ListQueryRequest<TRecord> request)
        where TRecord : class;
}

public interface IListRequestHandler<TRecord>
    where TRecord : class
{
    public ValueTask<Result<ListResult<TRecord>>> ExecuteAsync(ListQueryRequest<TRecord> request);
}
