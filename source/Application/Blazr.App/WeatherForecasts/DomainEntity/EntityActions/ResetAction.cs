/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed partial class WeatherForecastEntity
{
    /// <summary>
    /// Resets the Weather Forecast to the original state
    /// </summary>
    /// <returns></returns>
    public record ResetAction
    {
        public object? Sender { get; private init; }

        private ResetAction() { }

        public static ResetAction Create()
            => new() { Sender = null };

        public ResetAction WithSender(object sender)
            => this with { Sender = sender };

        public Result Execute(WeatherForecastEntity entity)
            => entity._weatherForecast.Reset(entity._baseWeatherForecast)
                .MapToResult(() => entity.ApplyRules(this.Sender))
                .SideEffect(() => entity.StateHasChanged?.Invoke(this.Sender, entity.Id));
    }
}
