/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

// Deletes the WeatherForecast
// If he delete fails, we roll back the last update and return the error
// And if the rules fail, we roll back the update and return the error
public sealed partial class WeatherForecastEntity
{
    public record DeleteWeatherForecastAction
    {
        public Guid TransactionId { get; private init; } = default!;
        public object? sender { get; private init; } = default!;

        private DeleteWeatherForecastAction() { }

        public Result<WeatherForecastEntity> Execute(WeatherForecastEntity entity)
            =>  entity._weatherForecast
                .MarkAsDeleted(this.TransactionId)
                .Map(
                    success: () => entity.StateHasChanged?.Invoke(this.sender, entity.WeatherForecast.Id),
                    failure: ex => entity._weatherForecast.RollBackLastUpdate(this.TransactionId)
                )
                .MapSuccess<WeatherForecastEntity>(() => Result<WeatherForecastEntity>.Success(entity));

        public static DeleteWeatherForecastAction Create()
            => new() { TransactionId = Guid.NewGuid() };

        public DeleteWeatherForecastAction AddSender(object? sender)
            => this with { sender = sender };

        public DeleteWeatherForecastAction AddTransactionId(Guid transactionId)
            => this with { TransactionId = transactionId };
    }
}
