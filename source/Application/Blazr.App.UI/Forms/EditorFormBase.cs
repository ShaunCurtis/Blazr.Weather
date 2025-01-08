/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public abstract class EditorFormBase<TRecord, TKey, TEditContext, TEntityService>
    : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull, IEntityId
    where TEditContext : class, IRecordEditContext<TRecord>, new()
    where TEntityService : class, IUIEntityService<TRecord>
{
    [Inject] protected IEditPresenterFactory<TEditContext, TKey> PresenterFactory { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected IJSRuntime Js { get; set; } = default!;
    [Inject] protected IUIEntityService<TRecord> UIEntityService { get; set; } = default!;

    [CascadingParameter] private IModalDialog? ModalDialog { get; set; }
    [Parameter] public TKey? Uid { get; set; }
    [Parameter] public bool LockNavigation { get; set; } = true;

    protected IEditPresenter<TEditContext, TKey> Presenter = default!;
    protected string exitUrl = "/";

    protected EditFormButtonsOptions editFormButtonsOptions = new();
    protected bool ExitOnSave = true;

    protected bool IsNewRecord => this.Presenter.CommandState == CommandState.Add;

    protected async override Task OnInitializedAsync()
    {
        await this.PresenterFactory.GetPresenterAsync(Uid);
        this.Presenter.EditContext.OnFieldChanged += OnEditStateMayHaveChanged;
    }

    protected async Task OnSave()
    {
        await this.Presenter.SaveItemAsync(!this.ExitOnSave);
        if (this.ExitOnSave)
            await OnExit();
    }

    protected async Task OnDelete()
    {
        bool confirmed = await Js.InvokeAsync<bool>("confirm", "Are you sure you want to delete this item?");
        if (!confirmed)
            return;

        await this.Presenter.DeleteItemAsync();
        if (this.ExitOnSave)
            await OnExit();
    }

    protected Task OnExit()
    {
        if (this.ModalDialog is null)
            this.NavManager.NavigateTo(this.exitUrl);

        ModalDialog?.Close(new ModalResult());
        return Task.CompletedTask;
    }

    protected void OnEditStateMayHaveChanged(object? sender, EventArgs e)
        => this.StateHasChanged();

    protected async Task OnReset()
    {
        await this.Presenter.ResetItemAsync();
        this.Presenter.EditContext.Validate();
    }

    public void Dispose()
    {
        this.Presenter.EditContext.OnFieldChanged -= OnEditStateMayHaveChanged;
    }
}
