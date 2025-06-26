/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.WeatherForecastActions;

namespace Blazr.App.Core;
public static partial class WeatherForecastActions
{
    public readonly record struct ResetWeatherForecastAction(object? sender);
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Forecast to the original state
    /// </summary>
    /// <returns></returns>
    public ValueTask<Result> DispatchAsync(ResetWeatherForecastAction action)
        => ValueTask.FromResult<Result>(ResetWeatherForecast(action));

    private Result ResetWeatherForecast(ResetWeatherForecastAction action)
    {
        _weatherForecast.Reset(_baseWeatherForecast);
        this.ApplyRules(ApplyRulesAction.Empty);
        this.StateHasChanged?.Invoke(action.sender, this.Id);

        return Result.Return();
    }
}
