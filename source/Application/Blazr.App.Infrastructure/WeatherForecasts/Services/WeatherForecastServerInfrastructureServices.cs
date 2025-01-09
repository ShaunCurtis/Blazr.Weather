/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public static class WeatherForecastServerInfrastructureServices
{
    public static void AddWeatherForecastServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IRecordIdProvider<WeatherForecastId, DmoWeatherForecast>, WeatherForecastIdProvider>();
        services.AddScoped<INewRecordProvider<DmoWeatherForecast>, NewWeatherForecastProvider>();
    }
}
