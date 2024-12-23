/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Presentation;

public static class GridStateExtensions
{
    public static int Page(this GridState state)
       => state.StartIndex / state.PageSize;
}

