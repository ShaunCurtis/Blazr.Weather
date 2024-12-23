﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.App.Core;

public record WeatherForecastListRequest
    : BaseListRequest, IRequest<ListQueryResult<DmoWeatherForecast>>
{
    public string? Summary { get; init; }
}
