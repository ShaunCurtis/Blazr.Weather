/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.Gallium;
using MediatR;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace Blazr.App.Infrastructure;

public class WeatherForecastAPICommandHandler :  IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
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

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<DmoWeatherForecast>>(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, request, cancellationToken);

        if (!httpResult.IsSuccessStatusCode)
            return Result<WeatherForecastId>.Fail(new CommandException($"The server returned a status code of : {httpResult.StatusCode}"));

        var result = await httpResult.Content.ReadFromJsonAsync<Result<WeatherForecastId>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);


        if (!result.HasSucceeded(out DboWeatherForecast? record))
            return result.ConvertFail<WeatherForecastId>();

        _messageBus.Publish<DmoWeatherForecast>(DboWeatherForecastMap.Map(record));

        return Result<WeatherForecastId>.Success(new WeatherForecastId(record.ID));
    }

    public async ValueTask<CommandResult> ExecuteAsync(CommandRequest<DmoWeatherForecast> request)
    {
        using var http = _httpClientFactory.CreateClient(AppDictionary.Common.WeatherHttpClient);

        var apiRequest = CommandAPIRequest<DmoWeatherForecast>.FromRequest(request);

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<DmoWeatherForecast>>(AppDictionary.WeatherForecast.WeatherForecastCommandAPIUrl, apiRequest, request.Cancellation);

        if (!httpResult.IsSuccessStatusCode)
            return CommandResult.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var commandAPIResult = await httpResult.Content.ReadFromJsonAsync<CommandAPIResult<Guid>>()
            .ConfigureAwait(ConfigureAwaitOptions.None);

        CommandResult? commandResult = null;

        if (commandAPIResult is not null)
            commandResult = commandAPIResult.ToCommandResult();

        return commandResult ?? CommandResult.Failure($"No data was returned");
    }
}
