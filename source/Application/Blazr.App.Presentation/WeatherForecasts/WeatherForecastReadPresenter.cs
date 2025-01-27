/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

/// <summary>
/// This object should not be used in DI.
/// Create an instance through the Factory
/// </summary>
public class WeatherForecastReadPresenter : ReadPresenter<DmoWeatherForecast, WeatherForecastId>
{
    public WeatherForecastReadPresenter(IMediator dataBroker, IEntityProvider<DmoWeatherForecast, WeatherForecastId> newRecordProvider) : base(dataBroker, newRecordProvider)  { }

    protected override Task<Result<DmoWeatherForecast>> GetItemAsync(WeatherForecastId id)
    {
        return _dataBroker.Send(new WeatherForecastItemRequest(id));
    }
}