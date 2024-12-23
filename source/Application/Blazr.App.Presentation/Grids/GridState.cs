/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public record GridState : IGridState
{
    public int PageSize { get; init; } = 1000;
    public int StartIndex { get; init; } = 0;
    public FilterDefinition? Filter { get; init; } = null;
    public SortDefinition? Sorter { get; init; } = null;

    public Guid Id { get; init; } = Guid.NewGuid();

    public GridState() { }

    public static GridState Create(Guid id)
        => new() { Id = id };
}

