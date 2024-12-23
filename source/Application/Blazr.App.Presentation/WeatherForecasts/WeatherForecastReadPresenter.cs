/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class WeatherForecastReadPresenter : ReadPresenter<DmoWeatherForecast, WeatherForecastId>
{
    public WeatherForecastReadPresenter(IMediator dataBroker, INewRecordProvider<DmoWeatherForecast> newRecordProvider) : base(dataBroker, newRecordProvider)  { }

    protected override Task<ItemQueryResult<DmoWeatherForecast>> GetItemAsync(WeatherForecastId id)
    {
        return _dataBroker.Send(new WeatherForecastItemRequest(id));
    }
}