/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public abstract class LookUpPresenter<TItem>
    : ILookUpPresenter<TItem>
        where TItem : class, IFkItem, new()
{
    protected IMediator DataBroker;

    public Task LoadTask { get; private set; } = Task.CompletedTask;

    public IEnumerable<TItem> Items { get; protected set; } = Enumerable.Empty<TItem>();

    public LookUpPresenter(IMediator dataBroker)
    {
        DataBroker = dataBroker;
        LoadTask = LoadAsync();
    }

    public abstract Task<bool> LoadAsync();

    public async void OnUpdate(object? sender, EventArgs e)
        => await this.LoadAsync();
}

