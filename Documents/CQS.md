# CQS and the Data Pipeline

The Blazor.Antimony package provides the basic infrastructure for implementing CQS.  You can read up about CQS elsewhere, so I'll assume you either alreeady know what CQS is, or have now acquainted yourself.

The data pipeline has three distinct pathways:

1. Commands - a Create/Update/Delete command to the datastore.
2. Record Query - a request for a single record based on its unique identifier.
3. List Query - a request for a paged collection of items with optional sorting and filtering.

## Domain Entities

The domain entities are defined in the `Blazr.App.Core` project.  The `DmoWeatherForecast` entity is defined as:

```csharp
public sealed record DmoWeatherForecast
{
    public WeatherForecastId Id { get; init; } = WeatherForecastId.Default;
    public Date Date { get; init; }
    public Temperature Temperature { get; init; }
    public string Summary { get; init; } = "[Not Defined]";
}
```

It's a record: immutable and sealed: there no reason to inherit from it.

The `Id` is a value object that wraps a `Guid`.  The `Date` and `Temperature` are also value objects that wrap a `DateOnly` and `Decimal` respectively.

## Entity Mapping

Well designed domain entities need mappers to move data out of and into the underlying data store objects.  There are various ways to implement this.  You can use a library, or build your own: I prefer to do my own.  It's not difficult as you will see, and you are in full control.

In the application the database mapped object is `DboWeatherForecast`.  Its based on primitives.

```csharp
public sealed record DboWeatherForecast : ICommandEntity
{
    [Key] public Guid ID { get; init; } = Guid.Empty;
    public DateTime Date { get; init; } = DateTime.MinValue; 
    public Decimal TemperatureC { get; init; } = decimal.MinValue;
    public string Summary { get; init; } = string.Empty;
}
```

Blazr.Antimony defines a mapping interface `IEntityMap` that you can implement to map between the domain entity and the database entity.

```csharp
public interface IDboEntityMap<TDboEntity, TDomainEntity>
{
    public TDomainEntity MapTo(TDboEntity item);
    public TDboEntity MapTo(TDomainEntity item);
}
```

The `DboWeatherForecastMap` implements the `IDboEntityMap` interface.  Note implementation of both instance and static maps.  Also note that the doamin entity to database object map detects a new record with a default ID and creates a new ID. 

```csharp
public class DboWeatherForecastMap : IDboEntityMap<DboWeatherForecast, DmoWeatherForecast>
{
    public DmoWeatherForecast MapTo(DboWeatherForecast item)
        => Map(item);

    public DboWeatherForecast MapTo(DmoWeatherForecast item)
        => Map(item);

    public static DmoWeatherForecast Map(DboWeatherForecast item)
        => new()
        {
            Id = new(item.ID),
            Date = new Date(item.Date),
            Temperature = new(item.TemperatureC),
            Summary = item.Summary
        };

    public static DboWeatherForecast Map(DmoWeatherForecast item)
        => new()
        {
            ID = item.Id.IsDefault ? WeatherForecastId.Create.Value  : item.Id.Value,
            Date = item.Date.ToDateTime,
            TemperatureC = item.Temperature.TemperatureC,
            Summary = item.Summary
        };
}
```

## Mediatr

Mediatr provides the link between the front end and the CQS backend.

The Mediatr request for getting a single item is defined as:

```csharp
public readonly record struct WeatherForecastItemRequest(
        WeatherForecastId Id) 
    : IRequest<ItemQueryResult<DmoWeatherForecast>>;
```

And the Mediatr handler is defined as:

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

