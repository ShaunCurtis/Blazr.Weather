/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public readonly record struct WeatherForecastId(Guid Value) : IEntityId
{
    public bool IsNew => this.Value == Guid.Empty;
    public bool IsDefault => this == Default;
    public bool IsDefaultOrNew => this.IsNew || this.IsDefault;

    public static WeatherForecastId Default => new(new Guid("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"));
    public static WeatherForecastId Create => new(Guid.CreateVersion7());
}
