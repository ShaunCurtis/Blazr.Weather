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
        services.AddSingleton<IUIEntityService<DmoWeatherForecast>, WeatherForecastUIEntityService>();
    }
}
