/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public class NewWeatherForecastProvider : INewRecordProvider<DmoWeatherForecast>
{
    public DmoWeatherForecast NewRecord()
    {
        return new DmoWeatherForecast()
        {
            Id = new(Guid.CreateVersion7()),
            Date = new(DateTime.Now)
        };
    }

    public DmoWeatherForecast DefaultRecord()
    {
        return new DmoWeatherForecast { Id = WeatherForecastId.Default };
    }
}

