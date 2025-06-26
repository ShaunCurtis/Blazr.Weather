/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.WeatherForecastActions;

namespace Blazr.App.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct UpdateWeatherForecastAction(DmoWeatherForecast Item, Guid TransactionId, object? sender);
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Updates the Weather Forecast record
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public ValueTask<Result> DispatchAsync(UpdateWeatherForecastAction action)
        => ValueTask.FromResult<Result>(UpdateWeatherForecast(action));

    // Updates the WeatherForecast
    // If the update fails, we roll back the last update and return the error
    // If it succeeds, we apply the rules
    // And if the rules fail, we roll back the update and return the error
    private Result UpdateWeatherForecast(UpdateWeatherForecastAction action)
        => _weatherForecast
            .Update(action.Item, action.TransactionId)
            .Bind(() => this.ApplyRules(new(action.sender)))
            .Map(
                success: () => this.StateHasChanged?.Invoke(action.sender, this.Id),
                failure: ex => _weatherForecast.RollBackLastUpdate(action.TransactionId)
            );
}
