/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public record Maybe<T> where T : class
{
    private readonly T? _value;

    private Maybe(T? value = null)
    {
        _value = value;
    }

    public Maybe<TOut> Map<TOut>(Func<T, Maybe<TOut>> func) where TOut : class
        => _value is null ? Maybe<TOut>.None() : func(_value);

    public Maybe<TOut> Map<TOut>(Func<T, TOut> func) where TOut : class
    {
        if (_value is null)
            return Maybe<TOut>.None();

        var result = func(_value);
        
        return result is null 
            ? Maybe<TOut>.None() 
            : new Maybe<TOut>(result);
    }

    public Maybe<T> Output(Action<T>? Yes = null, Action? No = null)
    {
        if (_value is not null && Yes != null)
            Yes(_value);

        if (_value is null && No != null)
            No();

        return this;
    }

    public static Maybe<T> Create(T? value)
        => value is null ? Maybe<T>.None() : Maybe<T>.Some(value);

    public static Maybe<T> None() => new Maybe<T>();

    public static Maybe<T> Some(T value)
        => new Maybe<T>(value);
}
