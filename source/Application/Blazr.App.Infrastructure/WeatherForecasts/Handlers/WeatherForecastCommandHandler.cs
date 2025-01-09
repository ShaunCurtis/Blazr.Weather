/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public record WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCommandRequest, Result<WeatherForecastId>>
{
    private ICommandHandler _handler;
    private IMessageBus _messageBus;

    public WeatherForecastCommandHandler(ICommandHandler handler, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _handler = handler;
    }

    public async Task<Result<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.ExecuteAsync<DboWeatherForecast>(new CommandRequest<DboWeatherForecast>(
            Item: DboWeatherForecastMap.Map(request.Item),
            State: request.State,
            Cancellation: cancellationToken
        ));

        if (!result.HasSucceeded(out DboWeatherForecast? record))
            return result.ConvertFail<WeatherForecastId>();

        _messageBus.Publish<DmoWeatherForecast>(DboWeatherForecastMap.Map(record));

        return Result<WeatherForecastId>.Success(new WeatherForecastId(record.ID));
    }
}
