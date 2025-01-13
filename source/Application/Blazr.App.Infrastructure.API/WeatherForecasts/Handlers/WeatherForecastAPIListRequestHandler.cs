/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPIListRequestHandler : IRequestHandler<WeatherForecastListRequest, Result<ListResult<DmoWeatherForecast>>>
{
    private readonly IHttpClientFactory _httpClientFactory;

    public WeatherForecastAPIListRequestHandler(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Result<ListResult<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var httpResult = await http.PostAsJsonAsync<WeatherForecastListRequest>(AppDictionary.WeatherForecast.WeatherForecastListAPIUrl, request, cancellationToken)
            .ConfigureAwait(ConfigureAwaitOptions.None);

        if (!httpResult.IsSuccessStatusCode)
            return Result<ListResult<DmoWeatherForecast>>.Fail(new ListQueryException( $"The server returned a status code of : {httpResult.StatusCode}"));

        var listResult = await httpResult.Content.ReadFromJsonAsync<Result<ListResult<DmoWeatherForecast>>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return listResult;
    }
}
