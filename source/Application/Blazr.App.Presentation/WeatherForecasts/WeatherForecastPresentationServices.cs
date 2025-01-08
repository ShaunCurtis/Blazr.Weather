/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public static class WeatherForecastPresentationServices
{
    public static void AddGroupPresentationServices(this IServiceCollection services)
    {
        services.AddTransient<IGridPresenter<DmoWeatherForecast>, WeatherForecastGridPresenter>();
        services.AddTransient<IEditPresenterFactory<WeatherForecastEditContext, WeatherForecastId>, WeatherForecastEditPresenterFactory>();
        services.AddTransient<IReadPresenterFactory<DmoWeatherForecast, WeatherForecastId>, WeatherForecastReadPresenterFactory>();
    }
}
