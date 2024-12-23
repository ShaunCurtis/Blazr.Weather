/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class AppPresentationServices
{
    public static void AddAppPresentationServices(this IServiceCollection services)
    {
        // GridState inMemory Store 
        services.AddScoped<KeyedFluxGateStore<GridState, Guid>>();
        services.AddTransient<FluxGateDispatcher<GridState>, GridStateDispatcher>();

        services.AddQuickGridEntityFrameworkAdapter();

        services.AddGroupPresentationServices();
    }
}
