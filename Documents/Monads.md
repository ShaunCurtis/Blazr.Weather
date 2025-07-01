# Monads

Monads are an enigma to many programmers.  There are a plethera of articles and publications that try to explain them but fail.

If you stick to you're OOP dogma, fail to open your mind, this will be another.


Consider the following console App:

```csharp
string? text = Console.ReadLine();

var resultOrException = Result<string>.Return(text);

var result = text.Trim() // Removes leading and trailing whitespace
    .Replace("  ", " ") // Replaces double spaces with a single space
    .ToUpper(); // Converts to uppercase

Console.WriteLine(result);
```

Null warnings, but the code compiles.  Press `<CTL>Z<Return>` and you will crash the program: we need to deal with nulls.  

In the real world, methods like `Trim` and `Replace` could return nulls.

Your code is littered with: 

```csharp
if (x is not null)
{
    // do work
}
else
{
    // Do something else
}
```

## Enter The Monad

We'll build and use a `Result<T>` nomad.  It's a variation on the common `Maybe<T>` and `Option<T>` nomads, and built for data pipelines where we want to flow the error up the pipeline. 

A result has two possible states:
- **Success**: The operation completed successfully
- **Failure**: The operation failed, and the result contains an error message. 

We can define a result as follows:

```csharp
public record Result<T>
{
    private readonly Exception? _exception;
    private readonly T? _value;
}
```

With three private constructors:

```csharp
private Result(T? value)
    => _value = value;

private Result(Exception? exception)
    => _exception = exception ?? new ResultException("An error occurred. No specific exception provided.");

private Result()
    => _exception = new ResultException("An error occurred. No specific exception was provided.");
 }
```

## Creating a `Result<T>`

The constructors are private to tightly control object creation.

For our case we can define a static method:

```csharp
public static Result<T> Return(T? value) => value is null ? new(new ResultException("T was null")) : new(value);
```

I'm sticking fairly closely to standard Monad nomenclature, so  using `Return`.  In practice you can call the method what you like.

We can then define:

```csharp
var result = Result<string>.Return(text);
```

The `Return` basic template is:

```csharp
    T > Monad<T>
```

There's a further three `Return` methods:

```csharp
public static Result<T> Return(T value) => new(value);
public static Result<T> Return(Exception exception) => new(exception);
public static Result<T> ReturnException(string message) => new(new ResultException(message));
```

## Working with `Result<T>`

Our console app can now look like this:

```csharp
string? text = Console.ReadLine();

var result = Result<string>.Return(text);

Console.WriteLine(result);
```

The next problem is handling the Monad in `Console.WriteLine`.

We access the result using `Match` methods.  The standard implementation looks like this:

```csharp
public void Match(Action<T> success, Action<Exception> failure)
{
    if (_exception is null)
        success(_value!);
    else
        failure(_exception!);
}
```

And the console app:

```csharp
string? text = Console.ReadLine();

var result = Result<string>.Return(text);

result.Match(
    success: value => Console.WriteLine($"Success: {value}"),
    failure: ex => Console.WriteLine($"Failure: {ex.Message}")
);
```

or even:

```csharp
string? text = Console.ReadLine();

Result<string>
    .Return(text)
    .Match(
        success: value => Console.WriteLine($"Success: {value}"),
        failure: ex => Console.WriteLine($"Failure: {ex.Message}")
    );
```

## Chaining

At this point, interesting, but there's no real savings.

Let's add two more static constructors:

```csharp
public static Result<T> Return(Exception exception) => new(exception);
public static Result<T> ReturnException(string message) => new(new ResultException(message));
```

Next we introduce `Map` which wraps a normal function into the Monad.  It can be represented like this:

```
(a->b) -> Monad<a> -> Monad<b>.
```

The basic `Map` in `Result<T>` looks like this:

```csharp
public Result<U> Map<U>(Func<T, U> func)
{
    if (_exception is not null)
        return Result<U>.Return(_exception);

    try
    {
        return new Result<U>(func(_value!));
    }
    catch (Exception ex)
    {
        return new Result<U>(ex);
    }
}
```

Note that `Map` can switch types, so `T->U` and `T->U` methods are both valid.

If the input `Result<T>` is in error, it is passed the `Result<T>` on to the caller.  Otherwise, it calls the provided `Func` in a `catch` and returns the result, or captures any generated exception and wraps it in a new `Result<T>`.

The original program can now be re-written used lambda expressions:

```csharp
string? text = Console.ReadLine();

Result<string>
    .Return(text)
    .Map(value => value.Trim())
    .Map(value => Regex.Replace(value, @"\s+", " "))
    .Map(value => value.ToUpper())
    .Match(
        success: value => Console.WriteLine($"Success: {value}"),
        failure: ex => Console.WriteLine($"Failure: {ex.Message}")
    );
```

Or we can define specific functions:

We could also define pattern methods:

```csharp
  string ReplaceSpaces(string value)
    => Regex.Replace(value, @"\s+", " ");

string Trim(string value)
   => value.Trim();

string ToUpper(string value)
   => value.ToUpper();
```

And the original program can now be re-written:

```csharp
string? text = Console.ReadLine();

Result<string>
    .Return(text)
    .Map(Trim)
    .Map(ReplaceSpaces)
    .Map(ToUpper)
    .Match(
        success: value => Console.WriteLine($"Success: {value}"),
        failure: ex => Console.WriteLine($"Failure: {ex.Message}")
    );
```

## Bind

The basic bind pattern is:

```csharp
(T->Monad(U)) -> Monad<T> -> Monad<U>.
```

So we could re-write `ReplaceSpaces` to output a `Result<string>`:    

```csharp
Result<string> ReplaceSpaces(string value)
   => Result<string>.Return( Regex.Replace(value, @"\s+", " ")P);
```

And add `Bind` to `Result<T>`:

```csharp
public Result<U> Bind<U>(Func<T, Result<U>> func)
{
    return _exception is null
        ? func(_value!)
        : Result<U>.Return(_exception!);
}
```
Note that `Bind` can switch types, so `T->Result<T>` and `T->Result<U?` methods are both valid.


And the original program can now be re-written:

```csharp
string? text = Console.ReadLine();

Result<string>
    .Return(text)
    .Map(Trim)
    .Bind(ReplaceSpaces)
    .Map(ToUpper)
    .Match(
        success: value => Console.WriteLine($"Success: {value}"),
        failure: ex => Console.WriteLine($"Failure: {ex.Message}")
    );
```


