/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public class DboWeatherForecastMap : IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>
{
    public DmoWeatherForecast MapTo(DboWeatherForecast item)
        => Map(item);

    public DboWeatherForecast MapTo(DmoWeatherForecast item)
        => Map(item);

    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
            Id = new(item.ID),
            Date = new Date(item.Date),
            Temperature = new(item.TemperatureC),
            Summary = item.Summary
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            ID = item.Id.IsDefault ? WeatherForecastId.Create.Value  : item.Id.Value,
            Date = item.Date.ToDateTime,
            TemperatureC = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
