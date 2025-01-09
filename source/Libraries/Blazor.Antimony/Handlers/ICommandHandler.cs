/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Antimony.Core;

public interface ICommandHandler
{
    public ValueTask<Result<TRecord>> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class;
}
