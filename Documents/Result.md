# Result

All actions in **Diode** return a result.

Any method that returns a `T`, returns a `Result<T>` and any method that returns a `void`, returns a `Result`.

A result has two possible states:
- **Success**: The operation completed successfully
- **Failure**: The operation failed, and the result contains an error message. 

We can define a result as follows:
```csharp
public record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;

    private Result(T? value)
        => _value = value;

    private Result(Exception? exception)
        => _exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

    private Result()
        => _exception = new ResultException("An error occurred. No specific exception provided.");
 }
```

And define three static methods to create a result:

```csharp
public static Result<T> Return(T value) => new(value);
public static Result<T> Return(Exception exception) => new(exception);
public static Result<T> ReturnException(string message) => new(new ResultException(message));
```

The naming convention for the methods is `Return` to follow classic *Monad* nomenclature.

We access the result using `Match` methods:

```csharp
public void Match(Action<T> success, Action<Exception> failure)
{
    if (_exception is null)
        success(_value!);
    else
        failure(_exception!);
}

public void MatchSuccess(Action<T> success)
{
    if (_exception is null)
        success(_value!);
}

public void MatchFailure(Action<Exception> failure)
{
    if (_exception is not null)
        failure(_exception!);
}
```
### `Result<T>` in Action

This is the Mediator `WeatherForecastRecordHandler`.  It passes a `RecordQueryRequest<DvoWeatherForecast>` object into the data pipeline and gets a `Result<DvoWeatherForecast>` returned.

```csharp
public async Task<Result<DmoWeatherForecast>> HandleAsync(WeatherForecastRecordRequest request, CancellationToken cancellationToken)
{
    var asyncResult = await _factory.CreateDbContext()
        .GetRecordAsync<DvoWeatherForecast>(new RecordQueryRequest<DvoWeatherForecast>(item => item.WeatherForecastID == request.Id.Value));
  
    return asyncResult.Bind<DmoWeatherForecast>(WeatherForecastMap.Map);
}
```

At this point, we need to convert the `DvoWeatherForecast` into a `DmoWeatherForecast`.

We use a `Bind` method to apply the mapping function.

Bind is a standard functional programming method that applies a function to the value contained in the result, and in the process transitions from a `Rrsult<T>` to a `Result<U>`.

If:
 - The source `Result<T>` is a failure, it returns the failure without applying the function.
 - The source `Result<T>` is a success, it attempts to execute `func` on the source result T value and return the result of `func` as a `Result<U>`.  The try is *defensive* coding to capture any possible exceptions.

```csharp
public Result<U> Bind<U>(Func<T, U> func)
{
    if (_exception is not null)
        return Result<U>.Return(_exception!);

    try
    {
        return Result<U>.Return(func(_value!));
    }
    catch (Exception ex)
    {
        return Result<U>.Return(ex);
    }
}
```

Basically, `Bind` is a way to chain operations on the result, allowing us to transform the value from `T` to `U` by applying the provided *T -> U* method while preserving the result's success or failure state.

