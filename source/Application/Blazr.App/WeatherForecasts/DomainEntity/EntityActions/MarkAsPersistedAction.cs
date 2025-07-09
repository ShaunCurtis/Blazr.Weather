using System;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

public partial class WeatherForecastEntity
{

    public record MarkAsPersistedAction
    {
        public object? Sender { get; private init; }

        private MarkAsPersistedAction() { }

        public static MarkAsPersistedAction Create()
            => new() { Sender = null };

        public MarkAsPersistedAction WithSender(object sender)
            => this with { Sender = sender };

        public Result Dispatch(WeatherForecastEntity entity)
            => entity._weatherForecast.MarkAsPersisted()
            .SideEffect(() => entity.StateHasChanged?.Invoke(this.Sender, entity.Id));
    }
}
