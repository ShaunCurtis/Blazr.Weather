/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public record WeatherForecastItemHandler : IRequestHandler<WeatherForecastItemRequest, ItemQueryResult<DmoWeatherForecast>>
{
    private IItemRequestHandler _handler;

    public WeatherForecastItemHandler(IItemRequestHandler handler)
    {
        _handler = handler;
    }

    public async Task<ItemQueryResult<DmoWeatherForecast>> Handle(WeatherForecastItemRequest request, CancellationToken cancellationToken)
    {
        Expression<Func<DboWeatherForecast, bool>> findExpression = (item) =>
            item.ID == request.Id.Value;

        var query = new ItemQueryRequest<DboWeatherForecast>(findExpression);

        var result = await _handler.ExecuteAsync<DboWeatherForecast>(query);

        if (!result.Successful)
            return ItemQueryResult<DmoWeatherForecast>.Failure(result.Exception!);

        var returnItem = DboWeatherForecastMap.Map(result.Item!);

        return ItemQueryResult<DmoWeatherForecast>.Success(returnItem);
    }
}
