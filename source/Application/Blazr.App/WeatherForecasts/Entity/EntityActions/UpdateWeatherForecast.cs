/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.WeatherForecastActions;

namespace Blazr.App.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct UpdateWeatherForecastAction(DmoWeatherForecast Item, Guid TransactionId);
}

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Updates the Weather Forecast record
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async ValueTask<Result> DispatchAsync(object? sender, UpdateWeatherForecastAction action)
    {
        return (await this.UpdateWeatherForecastAsync(action))
            .MapSuccess(() =>this.StateHasChanged?.Invoke(sender, this.Id));
    }

    private async ValueTask<Result> UpdateWeatherForecastAsync(UpdateWeatherForecastAction action)
    {
        var currentitem = this._weatherForecast;

        _weatherForecast.Update(action.Item, action.TransactionId);
        var result = await this.ApplyRulesAsync();

        if (result.IsSuccess)
            return Result.Success();

        //Update failed the rules so we need to roll back the change
        _item.Update(action.Item);
        return result;
    }
}
