/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface ILookUpPresenter<TItem>
    where TItem : class, IFkItem, new()
{
    public Task LoadTask { get; }

    public IEnumerable<TItem> Items { get; }

    public Task<bool> LoadAsync();
}
