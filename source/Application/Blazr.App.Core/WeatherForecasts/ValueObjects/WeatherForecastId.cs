/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsDefault => this == Default;
    public static WeatherForecastId Create => new(Guid.CreateVersion7());
    public static WeatherForecastId Default => new(Guid.Empty);
}
