/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Blazr.App.API;

public static class WeatherForecastApiServices
{
    public static void AddWeatherForecastApiEndpoints(this WebApplication app)
    {
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, GetListAsync);
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, GetItemAsync);
        app.MapPost(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, ExecuteCommandAsync);
    }

    internal static async Task<IResult> GetListAsync(
        WeatherForecastListRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }

    internal static async Task<IResult> GetItemAsync(
        WeatherForecastItemRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request);

        return Results.Ok(result);
    }

    internal static async Task<IResult> ExecuteCommandAsync(
        WeatherForecastCommandRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return Results.Ok(result);
    }
}
