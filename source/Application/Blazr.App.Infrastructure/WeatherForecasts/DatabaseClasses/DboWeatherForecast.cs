/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public sealed record DboWeatherForecast : ICommandEntity
{
    [Key] public Guid ID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; } = DateTime.MinValue; 
    public Decimal TemperatureC { get; init; } = decimal.MinValue;
    public string Summary { get; init; } = string.Empty;
}
