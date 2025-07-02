/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class NullableFunctionalExtensions
{
    public static Nullable<T> Match<T>(this Nullable<T> value, Action<T> IsNotNull, Action IsNull)
        where T : struct
    {
        if (value.HasValue)
            IsNotNull(value.Value);
        else
            IsNull();

        return value;
    }
}
