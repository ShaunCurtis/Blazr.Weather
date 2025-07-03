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

    public Maybe<TO> Bind<TO>(Func<T, Maybe<TO>> func) where TO : class
        => _value is null ? Maybe<TO>.None() : func(_value);

    public Maybe<TO> Map<TO>(Func<T, TO> func) where TO : class
    {
        if (_value is null)
            return Maybe<TO>.None();

        var result = func(_value);
        
        return result is null 
            ? Maybe<TO>.None() 
            : new Maybe<TO>(result);
    }

    public Maybe<T> Match(Action<T> Yes, Action No)
    {
        if (_value is null)
        {
            No();
            return this;
        }

        Yes(_value);
        return this;
    }

    public Maybe<T> MatchYes(Action<T> Yes)
    {
        if (_value is not null)
        Yes(_value);

        return this;
    }

    public Maybe<T> MatchNo(Action No)
    {
        if (_value is null)
            No();

        return this;
    }

    public static Maybe<T> None() => new Maybe<T>();

    public static Maybe<T> Return(T value)
        => new Maybe<T>(value);
}
