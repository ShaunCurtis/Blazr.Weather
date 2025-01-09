/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public interface IItemRequestHandler
{
    public ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest<TRecord> request)
        where TRecord : class;
}
