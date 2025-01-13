/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace Blazr.App.Infrastructure.API;

public class WeatherForecastAPIItemRequestHandler : IRequestHandler<WeatherForecastItemRequest, Result<DmoWeatherForecast>>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPIItemRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<DmoWeatherForecast>> Handle(WeatherForecastItemRequest request, CancellationToken cancellationToken)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var httpResult = await http.PostAsJsonAsync<WeatherForecastItemRequest>(AppDictionary.WeatherForecast.WeatherForecastItemAPIUrl, request, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return Result<DmoWeatherForecast>.Fail( new ItemQueryException($"The server returned a status code of : {httpResult.StatusCode}"));

        var listResult = await httpResult.Content.ReadFromJsonAsync<Result<DmoWeatherForecast>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult;
    }
}
