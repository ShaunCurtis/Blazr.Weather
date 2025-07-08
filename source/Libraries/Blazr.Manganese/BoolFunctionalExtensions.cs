/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolFunctionalExtensions
{
    public static void Output(this bool value, Action? isTrue = null, Action? isFalse = null)
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

    public static bool SideEffect(this bool value, Action? isTrue = null, Action? isFalse = null)
    {
        if (value && isTrue != null)
        {
            isTrue();
            return value;
        }

        if (!value && isFalse != null)
        {
            isFalse();
            return value;
        }
        return value;
    }

    public static Result<T> Map<T>(this bool value, Func<Result<T>> isTrue, Func<Result<T>>? isFalse)
    {
        if (value)
            return isTrue();

        if(!value && isFalse != null)
            return isFalse();

        return Result<T>.Failure("The bound bool was false");
    }

    public static Result<T> Map<T>(this bool value, Func<bool, Result<T>> mapping)
        => mapping(value);

    public static async Task<Result<T>> MapAsync<T>(this bool value, Func<Task<Result<T>>> isTrue, Func<Task<Result<T>>> isFalse)
    {
        if (value)
        {
            var resultTrue = await isTrue();
            return resultTrue;
        }

        var resultFalse = await isFalse();
        return resultFalse;
    }
}
