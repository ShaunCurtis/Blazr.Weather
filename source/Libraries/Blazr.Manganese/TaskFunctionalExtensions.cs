/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> MapTaskAsync<T, TOut>(this Task<Result<T>> task, Func<T, Task<Result<TOut>>> mapping)
    {
        var result = await task.HandleTaskCompletionAsync();

        return await result.MapResultAsync(mapping);
    }

    public static async Task OutputTaskAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
    {
        var result = await task.HandleTaskCompletionAsync();

        result.OutputResult(success: success, failure: failure);
    }

    public static async Task OutputTaskAsync<T>(this Task<Result<T>> task, Action<T> success)
    {
        var result = await task.HandleTaskCompletionAsync();

        result.OutputResult(success: success);
    }

    public static Task<Result<T>> TaskSideEffectAsync<T>(this Task<Result<T>> task, Action<T>? success = null, Action<Exception>? failure = null)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.ResultSideEffect(success, failure));

    public static Task<Result> MapTaskAsync<T>(this Task<Result<T>> task)
        => task.HandleTaskCompletionAsync().ContinueWith((t) => t.Result.MapResult());

    private static Task<Result<T>> HandleTaskCompletionAsync<T>(this Task<Result<T>> task)
    {
        // Function to check for task completion and wrap any exceptions into the Result
        Func<Task<Result<T>>, Result<T>> CheckForTaskException = (t) =>
        {
            return t.IsCompletedSuccessfully.Map<T>(
                isTrue: () => t.Result,
                isFalse: () => Result<T>.Failure(t.Exception
                    ?? new Exception("The Task failed to complete successfully")));
        };

        return task
            .ContinueWith(CheckForTaskException);
    }
}
