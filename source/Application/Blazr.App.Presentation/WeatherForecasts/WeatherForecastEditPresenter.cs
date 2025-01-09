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
public class WeatherForecastEditPresenter : EditPresenter<DmoWeatherForecast, WeatherForecastEditContext, WeatherForecastId>
{
    public WeatherForecastEditPresenter(IMediator mediator, IRecordIdProvider<WeatherForecastId> keyProvider, INewRecordProvider<DmoWeatherForecast> newRecordProvider)
        : base(mediator, keyProvider, newRecordProvider) { }

    protected override Task<Result<DmoWeatherForecast>> GetItemAsync()
    {
        return this.Databroker.Send(new WeatherForecastItemRequest(this.EntityId));
    }

    protected override Task<Result<WeatherForecastId>> UpdateAsync(DmoWeatherForecast record, CommandState state)
    {
        return Databroker.Send(new WeatherForecastCommandRequest(record, state));
    }
}

