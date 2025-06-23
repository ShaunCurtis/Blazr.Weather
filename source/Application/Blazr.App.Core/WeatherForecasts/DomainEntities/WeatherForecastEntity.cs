/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Diode;

namespace Blazr.App.Core;

public sealed partial class WeatherForecastEntity
{
    private readonly EntityState<DmoWeatherForecast> _weatherForecast;

    // The initial records so we can reset 
    private readonly DmoWeatherForecast _baseWeatherForecast;

    public DmoWeatherForecast WeatherForecast => _weatherForecast.Record;
    public StateRecord<DmoWeatherForecast> WeatherForecastRecord => _weatherForecast.AsRecord;

    public bool IsDirty
        => _weatherForecast.IsDirty;

    public event EventHandler<WeatherForecastId>? StateHasChanged;

    public WeatherForecastId Id => _weatherForecast.Record.Id;

    public WeatherForecastEntity(DmoWeatherForecast weatherForecast)
    {
        var isNew = weatherForecast.Id.IsDefault;

        _baseWeatherForecast = weatherForecast;

        // Create new MembershipContext for the Membership
        _weatherForecast = new(weatherForecast, isNew);

    }
}
