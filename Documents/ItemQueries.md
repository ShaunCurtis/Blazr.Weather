# Data Pipeline Item Queries

Item Queries retrieve a single item from the data store based on an identity key.

An item query request is defined by:

```csharp
public readonly record struct ItemQueryRequest<TRecord>
{
    public Expression<Func<TRecord, bool>> FindExpression { get; private init; }
    public CancellationToken Cancellation { get; private init; }

    public ItemQueryRequest(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
    {
        this.FindExpression = expression;
        this.Cancellation = cancellation ?? new(); 
    }
    public static ItemQueryRequest<TRecord> Create(Expression<Func<TRecord, bool>> expression, CancellationToken? cancellation = null)
        => new ItemQueryRequest<TRecord>(expression, cancellation ?? new());
}
```

The `ItemQueryRequest` is passed to an `IItemRequestHandler` defined as:

```csharp
public interface IItemRequestHandler
{
    public ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest<TRecord> request)
        where TRecord : class;
}
```

With a generic server side implementation:

```csharp
public sealed class ItemRequestServerHandler<TDbContext>
    : IItemRequestHandler
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<TDbContext> _factory;

    public ItemRequestServerHandler(IServiceProvider serviceProvider, IDbContextFactory<TDbContext> factory)
    {
        _serviceProvider = serviceProvider;
        _factory = factory;
    }

    public async ValueTask<ItemQueryResult<TRecord>> ExecuteAsync<TRecord>(ItemQueryRequest<TRecord> request)
        where TRecord : class
    {
        return await this.GetItemAsync<TRecord>(request);
    }

    private async ValueTask<ItemQueryResult<TRecord>> GetItemAsync<TRecord>(ItemQueryRequest<TRecord> request)
        where TRecord : class
    {
        using var dbContext = _factory.CreateDbContext();
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

        var record = await dbContext.Set<TRecord>()
            .FirstOrDefaultAsync(request.FindExpression)
            .ConfigureAwait(false);

        if (record is null)
            return ItemQueryResult<TRecord>.Failure(new ItemQueryException($"No record retrieved with the Key provided"));

        return ItemQueryResult<TRecord>.Success(record);
    }
}
```

Points:

1. We use an `IDbContextFactory` and *unit of work* `DbContexts` scoped to the execution method.  

1. Tracking is turned off.
 
The Handler returns a `ItemQueryResult<TRecord>`.  There are two incarnations:

```csharp
public sealed record ItemQueryResult<TRecord> : IDataResult
{
    public TRecord? Item { get; private init;}
    public bool Successful { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;

    public ItemQueryResult() { }

    //Maps and static methods.
}
```
