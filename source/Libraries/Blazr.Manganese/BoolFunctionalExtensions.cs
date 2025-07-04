/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class BoolFunctionalExtensions
{
    public static void Match(this bool value, Action isTrue, Action isFalse)
    {
        if (value)
            isTrue();
        else 
            isFalse();
    }
    public static void MatchTrue(this bool value, Action isTrue)
    {
        if (value)
            isTrue();
    }

    public static void MatchFalse(this bool value, Action isFalse)
    {
        if (!value)
            isFalse();
    }

    public static Result<T> BindToResult<T>(this bool value, Func<Result<T>> isTrue, Func<Result<T>> isFalse)
        where T : struct
    {
        if (value)
            return isTrue();

        return isFalse();
    }

    public static async ValueTask<Result<T>> BindToResultValueAsync<T>(this bool value, Func<ValueTask<Result<T>>> isTrue, Func<ValueTask<Result<T>>> isFalse)
    {
        if (value)
        {
            var resultTrue = await isTrue();
            return resultTrue;
        }

        var resultFalse = await isFalse();
        return resultFalse;
    }

    public static async Task<Result<T>> BindToResultAsync<T>(this bool value, Func<Task<Result<T>>> isTrue, Func<Task<Result<T>>> isFalse)
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
