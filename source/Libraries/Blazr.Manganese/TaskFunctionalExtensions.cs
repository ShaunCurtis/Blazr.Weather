/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static async Task<Result<TOut>> AndThenAsync<T, TOut>(this Task<Result<T>> inTask, Func<T, Task<Result<TOut>>> mapping)
    {
        var result = await inTask.HandleTaskCompletionAsync();

        var x =  result.SideEffect(
            success: value => { },
            failure: ex => { }
            );


        return result.Bind<TOut>(async (x) =>  await mapping(x));
    }

    public static async Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<T> success, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.SideEffect(
            success: success,
            failure: failure);
    }

    public static async Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<T> success)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.SideEffect(success);
    }

    public static async Task<Result<T>> SideEffectAsync<T>(this Task<Result<T>> task, Action<Exception> failure)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result
            .SideEffect(failure);
    }

    public static async Task<Result> MapToResultAsync<T>(this Task<Result<T>> task)
    {
        var result = await task.HandleTaskCompletionAsync();

        return result.MapToResult();
    }

    private static async Task<Result<T>> HandleTaskCompletionAsync<T>(this Task<Result<T>> task)
    {
        var result = await task;

        return task.IsCompletedSuccessfully.MapFalse<T>(
            isFalse: () => Result<T>.Return(task.Exception
                ?? new Exception("The Task failed to complete successfully")));
    }
}
