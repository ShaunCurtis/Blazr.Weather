/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IGridPresenter<TRecord>
    where TRecord : class, new()
{
    public Guid ContextUid { get; }
    public GridState GridState { get; }
    public IDataResult LastResult { get; }

    public void SetContext(Guid context);
    public ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync();
    public IDataResult DispatchGridStateChange(IFluxGateAction action);

}
