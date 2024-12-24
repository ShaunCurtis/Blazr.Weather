# Mutating and Validating Entities

The flaw in using records is that they are immutable.  This is a good thing for the most part.  However, there are times when you need to mutate the data, normally in an edit form context.

This problem is addressed by building a editable class that mirrors the record.  The editable class is mutable and can be used to edit the data.  When the data is saved, the editable class is converted back to the record.

First an interface to define the functionality:

```csharp
public interface IRecordEditContext<TRecord>
    where TRecord : class
{
    public TRecord BaseRecord { get; }
    public TRecord AsRecord { get; }
    public bool IsDirty { get; }

    public IDataResult Load(TRecord record);
    public void Reset();
    public void SetAsPersisted();
}
```

And then a base class to implement the interface:

```csharp
public abstract class BaseRecordEditContext<TRecord, TKey>
    where TRecord : class, new()
{
    public TRecord BaseRecord { get; protected set; } = new();

    public abstract TRecord AsRecord { get; }

    public BaseRecordEditContext()
    {
        this.Load(this.BaseRecord);
    }

    public BaseRecordEditContext(TRecord record)
    {
        this.Load(record);
    }

    public bool IsDirty => this.BaseRecord != this.AsRecord;

    public abstract IDataResult Load(TRecord record);

    public void Reset()
    {
        var record = this.BaseRecord;
        this.BaseRecord = new();
        this.Load(record);
    }

    public void SetAsPersisted()
    {
        var record = this.AsRecord;
        this.BaseRecord = new();
        this.Load(record);
    }
}
```

And finally the WeatherForcast entity implementation:

```csharp
public sealed class WeatherForecastEditContext : BaseRecordEditContext<DmoWeatherForecast, WeatherForecastId>, IRecordEditContext<DmoWeatherForecast>
{
    [TrackState] public string? Summary { get; set; }
    [TrackState] public decimal Temperature { get; set; }
    [TrackState] public DateTime? Date { get; set; }

    public WeatherForecastEditContext() : base() { }

    public WeatherForecastEditContext(DmoWeatherForecast record) : base(record) { }

    public override DmoWeatherForecast AsRecord =>
    this.BaseRecord with
    {
        Date = new(this.Date ?? DateTime.MinValue),
        Summary = this.Summary ?? string.Empty,
        Temperature = new(this.Temperature)
    };

    public override IDataResult Load(DmoWeatherForecast record)
    {
        if (!this.BaseRecord.Id.IsDefault)
            return DataResult.Failure("A record has already been loaded.  You can't overload it.");

        this.BaseRecord = record;

        this.Summary = record.Summary;
        this.Temperature = record.Temperature.TemperatureC;
        this.Date = record.Date.Value.ToDateTime(TimeOnly.MinValue);

        return DataResult.Success();
    }
}
```

We call `AsRecord` to get the record to save.

The `[TrackState]` attribute is used by the *EditStateTracker* component in the `EditForm` to track edit state and control form functionsality such as which buttons to display and locking navigation.

## Validation

The application uses *FluentValidation*.  The validator for the above EditContext is:

```csharp
public class WeatherForecastEditContextValidator : AbstractValidator<WeatherForecastEditContext>
{
    public WeatherForecastEditContextValidator()
    {
        this.RuleFor(p => p.Summary)
            .MinimumLength(3)
            .WithState(p => p);

        this.RuleFor(p => p.Date)
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Date must be in the future")
            .WithState(p => p);

        this.RuleFor(p => p.Temperature)
            .GreaterThanOrEqualTo(-60)
            .LessThanOrEqualTo(70)
            .WithState(p => p);
    }
}
```

The edit form looks like this:

```html
<EditForm EditContext=this.Presenter.EditContext OnValidSubmit=this.OnSave>

    <BlazrFluentValidator TRecord="WeatherForecastEditContext" TValidator="WeatherForecastEditContextValidator" />
    <BlazrEditStateTracker LockNavigation=this.LockNavigation />

    // Edit Controls here

</EditForm>
```
``