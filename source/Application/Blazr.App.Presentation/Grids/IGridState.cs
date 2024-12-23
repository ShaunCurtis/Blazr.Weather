/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public interface IGridState
{
    public int PageSize { get; }
    public int StartIndex { get;}
    public FilterDefinition? Filter { get; }
    public SortDefinition? Sorter { get;}
}

