/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public sealed record WeatherForecastUIEntityService : IUIEntityService<DmoWeatherForecast>
{
    public string SingleDisplayName { get; } = "Weather Forecast";
    public string PluralDisplayName { get; } = "WeatherForecasts";
    public Type? EditForm { get; } = typeof(WeatherForecastEditForm);
    public Type? ViewForm { get; } = typeof(WeatherForecastViewForm);
    public string Url { get; } = "/weatherforecast";
}
