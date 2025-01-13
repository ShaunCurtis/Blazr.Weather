/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public abstract partial class GridFormBase<TRecord, TKey> : ComponentBase, IDisposable
    where TRecord : class, new()
    where TKey : notnull
{
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ILogger<GridFormBase<TRecord, TKey>> Logger { get; set; } = default!;
    [Inject] protected IRecordIdProvider<TRecord, TKey> RecordIdProvider { get; set; } = default!;
    [Inject] protected IGridPresenter<TRecord> Presenter { get; set; } = default!;
    [Inject] protected IMessageBus MessageBus { get; set; } = default!;
    [Inject] protected IUIEntityService<TRecord> UIEntityService { get; set; } = default!;

    [Parameter] public string? FormTitle { get; set; }
    [Parameter] public Guid GridContextId { get; set; } = Guid.NewGuid();
    [Parameter] public int PageSize { get; set; } = 15;
    [Parameter] public bool ResetGridContext { get; set; }


    protected IModalDialog modalDialog = default!;
    protected QuickGrid<TRecord> quickGrid = default!;
    protected virtual string formTitle => this.FormTitle ?? $"List of {this.UIEntityService?.PluralDisplayName ?? "Items"}";

    protected PaginationState Pagination = new PaginationState { ItemsPerPage = 10 };
    protected FilterDefinition? DefaultFilter { get; set; } = null;

    protected async override Task OnInitializedAsync()
    {
        this.MessageBus.Subscribe<TRecord>(this.OnStateChanged);

        this.Presenter.SetContext(this.GridContextId);
        this.Pagination.ItemsPerPage = this.PageSize;
        if (ResetGridContext)
            this.Presenter.DispatchGridStateChange(new ResetGridAction(this, 0, this.PageSize, null, DefaultFilter));

        await Pagination.SetCurrentPageIndexAsync(this.Presenter.GridState.Page());

        // Make sure we yield so we have the first UI render 
        await Task.Yield();
        ArgumentNullException.ThrowIfNull(this.modalDialog);
        ArgumentNullException.ThrowIfNull(this.quickGrid);
    }

    public async ValueTask<GridItemsProviderResult<TRecord>> GetItemsAsync(GridItemsProviderRequest<TRecord> gridRequest)
    {
        //mutate the GridState
        var mutationAction = UpdateGridPagingAction.Create(gridRequest);
        var mutationResult = Presenter.DispatchGridStateChange(mutationAction);

        var result = await this.Presenter.GetItemsAsync();

        return result;
    }

    protected virtual async Task OnEditAsync(TKey id)
    {
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        ArgumentNullException.ThrowIfNull(this.UIEntityService.EditForm);

        await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
    }

    protected virtual async Task OnViewAsync(TKey id)
    {
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        ArgumentNullException.ThrowIfNull(this.UIEntityService.ViewForm);

        await modalDialog.ShowAsync(this.UIEntityService.ViewForm, options);
    }

    protected virtual async Task OnAddAsync()
    {
        var options = new ModalOptions();
        // we don't set UId

        ArgumentNullException.ThrowIfNull(this.UIEntityService.EditForm);

        await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
    }

    protected virtual Task OnDashboardAsync(TKey id)
    {
        this.NavManager.NavigateTo($"{this.UIEntityService.Url}/dash/{RecordIdProvider.GetValueObject(id)}");

        return Task.CompletedTask;
    }

    private void OnStateChanged(object? message)
    {
        this.InvokeAsync(quickGrid.RefreshDataAsync);
    }

    protected Task LogErrorMessageAsync(string message)
    {
        Logger.LogError(message);
        return Task.CompletedTask;
    }

    protected void LogErrorMessage(string message)
    {
        Logger.LogError(message);
    }

    public void Dispose()
    {
        this.MessageBus.UnSubscribe<TRecord>(this.OnStateChanged);
    }
}
