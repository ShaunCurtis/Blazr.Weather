/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.UI;

public sealed record GroupUIEntityService : IUIEntityService<DmoWeatherForecast>
{
    public string SingleDisplayName { get; } = "Weather Forecast";
    public string PluralDisplayName { get; } = "WeatherForecasts";
    public Type? EditForm { get; } = typeof(GroupEditForm);
    public Type? ViewForm { get; } = typeof(GroupViewForm);
    public string Url { get; } = "/weatherforecast";
}
