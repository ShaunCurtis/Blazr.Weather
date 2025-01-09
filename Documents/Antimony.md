# Antimony

**Antimony** is a low ambition *No Frills* library for building *CQS* based Data Pipelines.

It's designed [if you wish] to plug into *Mediatr*.

 It's a set of interfaces, definitions and base implementations.

It consists of three channels or paths:

1. List Queries rwturning a collection of `TRcords`.
2. Item Queries returning a single `TRecord`.
3. Commands issuing *Create/Update/Delete* instructions.

## `Result<T>`

Antimony implements a `Result<T>` which is used for all returns.

It's basic implementation is fairly standard.

```csharp
public readonly record struct Result<T>
{
    private T? _value { get; init; }
    private Exception? _error { get; init; }

    private Result(T value)
    {
        IsSuccess = true;
        _value = value;
        _error = null;
    }

    private Result(Exception error)
    {
        IsSuccess = false;
        _value = default;
        _error = error;
    }

    [MemberNotNullWhen(true, nameof(_value))]
    [MemberNotNullWhen(false, nameof(_error))]
    private bool IsSuccess { get; }
    private bool IsFailure => !IsSuccess;

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Fail(Exception error) => new(error);

    public static implicit operator Result<T>(T value) => Success(value);
}
```

However, everything is private but the static constructors.

You can access the results by the standard `Map` method.

```csharp
    public void Map(Action<T> onSuccess, Action<Exception> onFail)
    {
        if (this.IsSuccess)
            onSuccess(_value);
        else
            onFail(_error);
    }
```

It also implements two other output methods, `HasSucceeded` and `HasFailed`, to output the internal state:

```csharp
    public bool HasSucceeded([NotNullWhen(true)] out T? item)
    {
        if (this.IsSuccess)
            item = _value;
        else
            item = default;

        return this.IsSuccess;
    }

    public bool HasFailed([NotNullWhen(true)] out Exception? exception)
    {
        if (this.IsFailure)
            exception = _error;
        else
            exception = default;

        return this.IsFailure;
    }
```