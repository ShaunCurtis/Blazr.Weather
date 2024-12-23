/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public static class ApplicationUIServices
{
    public static void AddAppUIServices(this IServiceCollection services)
    {
        services.AddSingleton<IUIEntityService<DcoGroup>, GroupUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoFarm>, FarmUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoFeed>, FeedUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoMix>, MixUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoFeedNutrient>, FeedNutrientUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoNutrient>, NutrientUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoFeedPrice>, FeedPriceUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoContract>, ContractUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoContractFeed>, ContractFeedUIEntityService>();
        services.AddSingleton<IUIEntityService<DcoContractMix>, ContractMixUIEntityService>();
        //services.AddSingleton<IUIEntityService<DcoDelivery>, DeliveryUIEntityService>();
        //services.AddSingleton<IUIEntityService<DcoContractFeedUsage>, ContractFeedUsageUIEntityService>();
    }
}
