/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public abstract partial class GridFormBase<TRecord, TKey> : BlazrControlBase
    where TRecord : class, new()
    where TKey : notnull
{
    [Inject] protected IServiceProvider ServiceProvider { get; set; } = default!;
    [Inject] protected NavigationManager NavManager { get; set; } = default!;
    [Inject] protected ILogger<GridFormBase<TRecord, TKey>> Logger { get; set; } = default!;
    [Inject] protected IRecordIdProvider<TKey> RecordIdProvider { get; set; } = default!;
    [Inject] protected IGridPresenter<TRecord> Presenter { get; set; } = default!;

    [Parameter] public string? FormTitle { get; set; }
    [Parameter] public Guid GridContextId { get; set; } = Guid.NewGuid();
    [Parameter] public int PageSize { get; set; } = 15;
    [Parameter] public bool ResetGridContext { get; set; }

    protected IUIEntityService<TRecord>? UIEntityService { get; set; }


    protected IModalDialog? modalDialog;
    protected QuickGrid<TRecord>? quickGrid;
    protected virtual string formTitle => this.FormTitle ?? $"List of {this.UIEntityService?.PluralDisplayName ?? "Items"}";

    protected PaginationState Pagination = new PaginationState { ItemsPerPage = 10 };
    protected FilterDefinition? DefaultFilter { get; set; } = null;

    protected async override Task OnParametersSetAsync()
    {
        if (NotInitialized)
        {
            // Loads the UI Entity service - we don't inject as we don't need it for basic display only forms.
            this.UIEntityService = this.ServiceProvider.GetService<IUIEntityService<TRecord>>();

            this.Presenter.SetContext(this.GridContextId);
            this.Pagination.ItemsPerPage = this.PageSize;
            if (ResetGridContext)
                this.Presenter.DispatchGridStateChange(new ResetGridAction(this, 0, this.PageSize, null, DefaultFilter));

            await Pagination.SetCurrentPageIndexAsync(this.Presenter.GridState.Page());
        }
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

        if (modalDialog is not null && this.UIEntityService is not null && this.UIEntityService.EditForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
            this.StateHasChanged();
        }

        else if (this.UIEntityService is not null)
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/edit/{RecordIdProvider.GetValueObject(id)}");
    }

    protected virtual async Task OnViewAsync(TKey id)
    {
        var options = new ModalOptions();
        options.ControlParameters.Add("Uid", id);

        if (modalDialog is not null && this.UIEntityService is not null && this.UIEntityService.ViewForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.ViewForm, options);
            this.StateHasChanged();
        }
        else if (this.UIEntityService is not null)
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/view/{RecordIdProvider.GetValueObject(id)}");
    }

    protected virtual async Task OnAddAsync()
    {
        var options = new ModalOptions();
        // we don't set UId

        if (modalDialog is not null && this.UIEntityService is not null && this.UIEntityService.EditForm is not null)
        {
            await modalDialog.ShowAsync(this.UIEntityService.EditForm, options);
            this.StateHasChanged();
        }
        else if (this.UIEntityService is not null)
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/edit/");
    }

    protected virtual Task OnDashboardAsync(TKey id)
    {
        if (this.UIEntityService is not null)
            this.NavManager.NavigateTo($"{this.UIEntityService.Url}/dash/{RecordIdProvider.GetValueObject(id)}");

        return Task.CompletedTask;
    }

    private void OnStateChanged(object? sender, EventArgs e)
    {
        quickGrid?.RefreshDataAsync();
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
}
