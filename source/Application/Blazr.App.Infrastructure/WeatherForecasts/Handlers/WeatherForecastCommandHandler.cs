/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public record WeatherForecastCommandHandler : IRequestHandler<WeatherForecastCommandRequest, CommandResult<WeatherForecastId>>
{
    private ICommandHandler _handler;
    private IMessageBus _messageBus;

    public WeatherForecastCommandHandler(ICommandHandler handler, IMessageBus messageBus)
    {
        _messageBus = messageBus;
        _handler = handler;
    }

    public async Task<CommandResult<WeatherForecastId>> Handle(WeatherForecastCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _handler.ExecuteAsync<DboWeatherForecast>(new CommandRequest<DboWeatherForecast>(
            Item: DboWeatherForecastMap.Map(request.Item),
            State: request.State,
            Cancellation: cancellationToken
        ));

        if (result.KeyValue is DboWeatherForecast record)
        {
            _messageBus.Publish<DmoWeatherForecast>(DboWeatherForecastMap.Map(record));
            return CommandResult<WeatherForecastId>.SuccessWithKey(new WeatherForecastId(record.ID));
        }
        return CommandResult<WeatherForecastId>.Failure(new CommandException($"Returned object was not a DmoWeatherForecast"));
    }
}
