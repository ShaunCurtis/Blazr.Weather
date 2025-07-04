using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> BindAsync<T, TOut>(this Task<Result<T>> task, Func<T, Result<TOut>> map)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.Bind<TOut>(map);
    }

    public static async Task<Result<T>> MatchAsync<T>(this Task<Result<T>> task, Action<T> success, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.Match(
            success: success,
            failure: failure);
    }

    public static async Task<Result<T>> MatchSuccessAsync<T>(this Task<Result<T>> task, Action<T> success)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.MatchSuccess(success);
    }

    public static async Task<Result<T>> MatchFailureAsync<T>(this Task<Result<T>> task, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result
            .MatchFailure(failure);
    }

    public static async Task<Result> MapToResultAsync<T>(this Task<Result<T>> task)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.MapToResult();
    }

    private static async Task<Result<T>> HandleTaskCompletionAsync<T>(this Task<Result<T>> task)
    {
        var result = await task;

        return task.IsCompletedSuccessfully.BindFalse<T>(
            isFalse: () => Result<T>.Return(task.Exception
                ?? new Exception("The Task failed to complete successfully")));
    }
}
