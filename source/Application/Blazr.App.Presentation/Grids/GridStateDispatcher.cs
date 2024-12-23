/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public readonly record struct UpdateGridFiltersAction(object Sender, FilterDefinition Filter) : IFluxGateAction;

public readonly record struct ResetGridAction(object Sender, int StartIndex, int PageSize, SortDefinition? Sorter, FilterDefinition? Filter) : IFluxGateAction;

public readonly record struct UpdateGridPagingAction(object Sender, int StartIndex, int PageSize, SortDefinition? Sorter) : IFluxGateAction
{
    public static UpdateGridPagingAction Create<TRecord>(GridItemsProviderRequest<TRecord> request)
    {
        List<SortDefinition> sortDefinitions = new();

        var column = request.SortByColumn as DataSetPropertyColumn<TRecord>;

        if (column is not null)
            sortDefinitions.Add(new(column.DataSetName, !request.SortByAscending));
        else
        {
            var definedSorters = request.GetSortByProperties();
            if (definedSorters is not null)
                sortDefinitions = definedSorters.Select(item => new SortDefinition(SortField: item.PropertyName, SortDescending: item.Direction == SortDirection.Descending)).ToList();
        }

        return new()
        {
            StartIndex = request.StartIndex,
            PageSize = request.Count ?? 0,
            Sorter = sortDefinitions.FirstOrDefault(),
        };
    }
}

public class GridStateDispatcher : FluxGateDispatcher<GridState>
{
    public override FluxGateResult<GridState> Dispatch(FluxGateStore<GridState> store, IFluxGateAction action)
    {
        return action switch
        {
            UpdateGridFiltersAction a1 => Mutate(store, a1),
            UpdateGridPagingAction a2 => Mutate(store, a2),
            ResetGridAction a3 => Mutate(store, a3),
            _ => new FluxGateResult<GridState>(false, store.Item, store.State)
        };
    }

    private static FluxGateResult<GridState> Mutate(FluxGateStore<GridState> store, UpdateGridFiltersAction action)
    {
        var updatedItem = store.Item with { Filter = action.Filter };
        var state = store.State.Modified();

        return new FluxGateResult<GridState>(true, updatedItem, state);
    }

    private static FluxGateResult<GridState> Mutate(FluxGateStore<GridState> store, UpdateGridPagingAction action)
    {
        var updatedItem = store.Item with { PageSize = action.PageSize, StartIndex = action.StartIndex, Sorter = action.Sorter };
        var state = store.State.Modified();

        return new FluxGateResult<GridState>(true, updatedItem, state);
    }

    private static FluxGateResult<GridState> Mutate(FluxGateStore<GridState> store, ResetGridAction action)
    {
        var updatedItem = store.Item with { PageSize = action.PageSize, StartIndex = action.StartIndex, Sorter = action.Sorter, Filter = action.Filter };
        var state = store.State.Modified();

        return new FluxGateResult<GridState>(true, updatedItem, state);
    }
}
