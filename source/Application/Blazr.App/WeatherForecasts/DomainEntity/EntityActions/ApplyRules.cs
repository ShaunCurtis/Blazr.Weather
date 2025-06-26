/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using static Blazr.App.Core.WeatherForecastActions;

namespace Blazr.App.Core;

public static partial class WeatherForecastActions
{
    public readonly record struct ApplyRulesAction(object? sender = null)
    {
        public static ApplyRulesAction Empty => new ApplyRulesAction();
    }
}

public sealed partial class WeatherForecastEntity
{
    private bool _processing;

    /// <summary>
    /// Applies the business rules to the Weather Forecast
    /// </summary>
    /// <param name="sender"></param>
    public ValueTask<Result> DispatchAsync(ApplyRulesAction action)
        => ValueTask.FromResult(ApplyRules(action));

    private Result ApplyRules(ApplyRulesAction action)
        => SetProcessing()
            .Bind(RunRules)
            .MapSuccess(() => this.StateHasChanged?.Invoke(action.sender ?? this, this.Id)
        );

    private Result RunRules()
    {
        var result = NewDateNotInTheFutureRule();
        _processing = false;

        return result;
    }

    private Result SetProcessing()
    {
        var result = _processing
            ? Result.ReturnException("Rules already running.")
            : Result.Return();

        _processing = true;
        return result;
    }

    private Result NewDateNotInTheFutureRule()
        => (_weatherForecast.State == EditState.New && _weatherForecast.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            ? Result.Return(new ValidationException("A new weather forecast must have a future date."))
            : Result.Return();
}
