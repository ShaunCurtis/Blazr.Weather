# Functional Programming in C#

Applying the Functional Programming [FP from now on] paradigm to Object Oriented Programming [OOP] languages such as C# requires compromises on both sides.  There are some fundimental concepts that don't sit well on the other side.

This article is my take on melding FP into C# to take advantage of the benefits.

## `Result<T>` and `Result`

`Result<T>` and `Result` are the foundation stones of my FP implementation - as `Task` and `Task<T>` are to async coding.

Any method that would return a value:

```csharp
public int ToInt(string value) {..}
```

returns a `Result<T>`:
 
```csharp
public Result<int> ToInt(string value) {..}
```

Any method that would return a `void`:

```csharp
public void DoSomething(string value) {..}
```

returns a `Result`:

```csharp
public Result DoSomething(string value) {..}
```

There's a separate article dealing with `Result<T>` and `Result`, so I'll only cover the basics here.

A result has two possible states:

- **Success**: The operation completed successfully
- **Failure**: The operation failed, and the result contains an `Exception` or an error message wrapped in an `ResultException`. 

The basic definitions are as follows:

```csharp
public record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;
    private ResultException _defaultException => new ResultException("An error occurred. No specific exception provided.");

    private Result(T? value) 
        => _value = value;

    private Result(Exception? exception) 
        => _exception = exception ?? _defaultException;

    private Result() 
        => _exception = _defaultException;

    public static Result<T> Create(T? value) => 
        value is null
            ? new(new ResultException("T was null."))
            : new(value);

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Exception exception) => new(exception);

    public static Result<T> Failure(string message) => new(new ResultException(message));

    //... functional methods
}
```

```csharp
public record Result
{
    private readonly Exception? _exception;

    private Result(Exception? exception)
        => _exception = exception
            ?? new ResultException("An error occurred. No specific exception provided.");

    private Result() { }

    public static Result Success() => new();
    public static Result Failure(Exception? exception) => new(exception);
    public static Result Failure(string message) => new(new ResultException(message));

    //... functional methods
}
```

The real power of Result comes when we add FP methods.  Before we start on those, we need to cover some more basics.

## Expressions and Statements

Consider this code:

```csharp
int x = 0;

if(someBoolCondition is true)
    x = 2;
else
    x = 3;
```

`int x = 0;` is an **expression**, but it only sets a default value.  The real assignment is in the `if.. else..` **statement** block.

Modern C# has added this *syntatic sugar*:

```csharp
int x = somecondition is true ? 2 : 3;
```

But this only works for simply one liners.

This doesn't compile:

```csharp
int x = someBoolCondition is true
    ? { return 2;} 
    : { return 3;};
```

### Boolean Extensions

We can start to solve this problem by adding some FP to `bool`:

You could create a True/False mapper like this:

```csharp
public static T Map<T>(this bool value, Func<T> isTrue, Func<T> isFalse)
    => value ? isTrue() : isFalse();
```

And used like this:

```csharp
int x = someBoolCondition.Map(
    isTrue: 
        {
            // Do some work
            return 2;
        },
    isFalse: 
        {
            // Do some work
            return 3;
        }
);
```

But in my FP world all functions return a result, so `Map` looks like this:

```csharp
    public static Result<T> Map<T>(this bool value, Func<Result<T>> isTrue, Func<Result<T>>? isFalse)
    {
        if (value)
            return isTrue();

        if(!value && isFalse != null)
            return isFalse();

        return Result<T>.ReturnException("The bound bool was false");
    }
```

```csharp
int x = someBoolCondition.Map((value) => value? 2 
);
```




```csharp
public static void Match(this bool value, Action? isTrue = null, Action? isFalse = null)
{
    if (value && isTrue != null)
    {
        isTrue();
        return;
    }

    if (!value && isFalse != null)
    {
        isFalse();
        return;
    }
    return;
}
```
