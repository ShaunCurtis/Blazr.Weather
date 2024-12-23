/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.FluxGate;

namespace Blazr.App.Core;

public static class FluxGateExtensions
{
    public static CommandState ToCommandState(this FluxGateState state)
    {
        if (state.IsNew)
            return CommandState.Add;
        if (state.IsDeleted)
            return CommandState.Delete;
        if (state.IsModified)
            return CommandState.Update;

        return CommandState.None;
    }

    public static IDataResult ToDataResult<TFluxItem>(this FluxGateResult<TFluxItem> result)
        => new DataResult() { Successful = result.Success, Message = result.Message };
}
