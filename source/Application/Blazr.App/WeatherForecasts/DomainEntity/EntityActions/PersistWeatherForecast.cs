/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.WeatherForecastActions;

namespace Blazr.App.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct PersistWeatherForecastAction(object? sender)
    {
        public static PersistWeatherForecastAction Empty => new();
    }
}

public sealed partial class WeatherForecastEntity
{
    public async ValueTask<Result> DispatchAsync(PersistWeatherForecastAction action)
    {
        return (await _mediatorBroker.Send(new WeatherForecastCommandRequest(_weatherForecast.Record, _weatherForecast.State)))
            .MapSuccess((id) => _weatherForecast.MarkAsPersisted())
            .MapSuccess(() => this.StateHasChanged?.Invoke(action.sender, this.Id));
    }
}
