# CQS and the Data Pipeline

The Blazor.Antimony package provides the basic infrastructure for impkementing CQS.  You can read up about CQS elsewhere, so I'll assume you either alreeady know what CQS is, or have now acquainted yourself.

The data pipeline are three distinct pathways:

1. Commands - a Create/Update/Delete command to the datastore.
2. Item Query - a request for a single item based on their unique identifier
3. List Query - a request for a paged collection of items with optional sorting and filtering.

## Entity Mapping

## Mediatr

```csharp
public readonly record struct WeatherForecastItemRequest(
        WeatherForecastId Id) 
    : IRequest<ItemQueryResult<DmoWeatherForecast>>;
```

```csharp
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
```