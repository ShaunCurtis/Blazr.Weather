/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using System.Net.Http.Json;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPICommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMessageBus _messageBus;

    public WeatherForecastAPICommandHandler(IHttpClientFactory httpClientFactory, IMessageBus messageBus)
    {
        _httpClientFactory = httpClientFactory;
        _messageBus = messageBus;
    }

    public async Task<Result<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var httpResult = await http.PostAsJsonAsync<WeatherForecastCommandRequest>(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, request, cancellationToken);

        if (!httpResult.IsSuccessStatusCode)
            return Result<WeatherForecastId>.Fail(new CommandException($"The server returned a status code of : {httpResult.StatusCode}"));

        Result<WeatherForecastId> result = await httpResult.Content.ReadFromJsonAsync<Result<WeatherForecastId>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        return result;
    }
}
