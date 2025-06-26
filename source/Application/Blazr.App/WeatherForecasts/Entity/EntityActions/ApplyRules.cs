/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Diagnostics.CodeAnalysis;
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
    public async ValueTask<Result> DispatchAsync(object? sender, ApplyRulesAction action)
    {
        return (await this.ApplyRulesAsync(action))
            .MapSuccess(() => this.StateHasChanged?.Invoke(sender, this.Id));
    }

    private ValueTask<Result> ApplyRulesAsync(ApplyRulesAction? action = null)
    {
        var x = SetProcessing()
            .Bind(NewDateNotInTheFutureRule);
 
        // Don't process if already processing
        if (_processing)
            return ValueTask.FromResult(Result.ReturnException("Rules already running."));

        _processing = true;

        // apply rules
        var result = this.TryNewDateNotInTheFutureRule();
            .Map(
            success: () =>
            {
                _processing = false;
            },
            failure: (exception) =>
            {
                _processing = false;
            });

        if (this.TryNewDateNotInTheFutureRule(out ValidationException? exception))
            return ValueTask.FromResult(Result.Fail(exception!));

        // Notify any listeners that the state has changed
        this.StateHasChanged?.Invoke(action?.sender ?? this, this.Id);

        _processing = false;

        return ValueTask.FromResult(Result.Success());
    }

    private Result SetProcessing()
    {
        var result = _processing
            ? Result.ReturnException("Rules already running.")
            : Result.Return() ;
        _processing = true;
        return result;
    }
    private Result SetComplete()
    {
        _processing = false;
        return  Result.Return();
    }

    private Result NewDateNotInTheFutureRule()
        => (_weatherForecast.State == EditState.New && _weatherForecast.Record.Date.Value > DateOnly.FromDateTime(DateTime.Now))
            ? Result.Return(new ValidationException("A new weather forecast must have a future date."))
            : Result.Return();
}
