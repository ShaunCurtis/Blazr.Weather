/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> AndThenAsync<T, TOut>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> mapping)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.MapAsync(mapping);
    }
    
    public static async Task MatchAsync<T>(this Task<Result<T>> task, Action<T> success, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        result.Match(success, failure);
    }

    public static async Task MatchAsync<T>(this Task<Result<T>> task, Action<T> success)
    {
        var result = await task.HandleTaskCompletionAsync();

        result.Match(success);
    }

    public static async Task MatchAsync<T>(this Task<Result<T>> task, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        result.Match(failure);
    }

    public static Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<T> success, Action<Exception> failure)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.SideEffect(success, failure));

    public static Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<T> success)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.SideEffect(success));

    public static Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<Exception> failure)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.SideEffect(failure));

    public static Task<Result> MapToResultAsync<T>(this Task<Result<T>> task)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.MapToResult());

    private static Task<Result<T>> HandleTaskCompletionAsync<T>(this Task<Result<T>> task)
    {
        // Function to check for task completion and wrap any exceptions into the Result
        Func<Task<Result<T>>, Result<T>> CheckForTaskException = (t) =>
        {
            return t.IsCompletedSuccessfully.Map<T>(
                isTrue: () => t.Result,
                isFalse: () => Result<T>.Return(t.Exception
                    ?? new Exception("The Task failed to complete successfully")));
        };

        return task
            .ContinueWith(CheckForTaskException);
    }
}
