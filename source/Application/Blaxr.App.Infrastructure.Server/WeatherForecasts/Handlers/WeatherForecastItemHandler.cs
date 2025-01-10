using Blazr.App.Core;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

public record WeatherForecastItemHandler : IRequestHandler<WeatherForecastItemRequest, Result<DmoWeatherForecast>>
{
    private IItemRequestHandler _handler;

    public WeatherForecastItemHandler(IItemRequestHandler handler)
    {
        _handler = handler;
    }

    public async Task<Result<DmoWeatherForecast>> Handle(WeatherForecastItemRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DboWeatherForecast, bool>> findExpression = (item) =>
            item.ID == request.Id.Value;

        var query = new ItemQueryRequest<DboWeatherForecast>(findExpression);

        var result = await _handler.ExecuteAsync<DboWeatherForecast>(query);

        if (!result.HasSucceeded(out DboWeatherForecast? record))
            return result.ConvertFail<DmoWeatherForecast>();

        var returnItem = DboWeatherForecastMap.Map(record);

        return Result<DmoWeatherForecast>.Success(returnItem);
    }
}
