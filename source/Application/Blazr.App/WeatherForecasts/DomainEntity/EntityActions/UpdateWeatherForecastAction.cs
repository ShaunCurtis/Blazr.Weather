/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

// Updates the WeatherForecast
// If the update fails, we roll back the last update and return the error
// If it succeeds, we apply the rules
// And if the rules fail, we roll back the update and return the error
public sealed partial class WeatherForecastEntity
{
    public record UpdateWeatherForecastAction
    {
        public DmoWeatherForecast Item { get; private init; } = default!;
        public Guid TransactionId { get; private init; } = default!;
        public object? sender { get; private init; } = default!;

        private UpdateWeatherForecastAction() { }

        public Result<WeatherForecastEntity> ExecuteAction(WeatherForecastEntity entity)
            =>  entity._weatherForecast
                .Update(this.Item, this.TransactionId)
                .Map(() => entity.ApplyRules(this.sender))
                .SideEffect(
                    success: () => entity.StateHasChanged?.Invoke(this.sender, this.Item.Id),
                    failure: ex => entity._weatherForecast.RollBackLastUpdate(this.TransactionId)
                )
                .Map<WeatherForecastEntity>(() => Result<WeatherForecastEntity>.Success(entity));

        public static UpdateWeatherForecastAction CreateAction(DmoWeatherForecast item)
            => new() { Item = item, TransactionId = Guid.NewGuid() };

        public UpdateWeatherForecastAction AddSender(object? sender)
            => this with { sender = sender };

        public UpdateWeatherForecastAction AddTransactionId(Guid transactionId)
            => this with { TransactionId = transactionId };
    }
}
