/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IReadPresenterFactory<TRecord, TKey>
    where TRecord : class, new()
    where TKey : notnull, IEntityId
{
    public ValueTask<IReadPresenter<TRecord, TKey>> GetPresenterAsync(TKey id);
}