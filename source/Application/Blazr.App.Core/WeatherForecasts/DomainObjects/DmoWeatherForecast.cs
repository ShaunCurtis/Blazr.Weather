﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Core;

[APIInfo(pathName: "WeatherForecast", clientName: AppDictionary.Common.WeatherHttpClient)]
public sealed record DmoWeatherForecast
{
    public WeatherForecastId Id { get; init; } = new(Guid.Empty);
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "Not Defined";
}
