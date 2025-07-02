/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Manganese;

public static class TaskFunctionalExtensions
{
    public static Task<T> Map<R, T>(this Task<R> task, System.Func<R, T> f)
        => task.ContinueWith(r => f(task.Result));

    public static async Task<T> Bind<R, T>(this Task<R> task, Func<R, Task<T>> f)
    {
        var r = await task;
        return await f(r);
    }

    public static async ValueTask<T> Bind<R, T>(this ValueTask<R> task, Func<R, ValueTask<T>> f)
    {
        var r = await task;
        return await f(r);
    }

    public static async Task Bind<R>(this Task<R> task, Func<R, Task> f)
    {
        var r = await task;
        await f(r);
        return;
    }

    public static async Task<T> Bind<T>(this Task task, Func<Task<T>> f)
    {
        await task;
        return await f();
    }

    public static async ValueTask<T> Bind<T>(this ValueTask task, Func<ValueTask<T>> f)
    {
        await task;
        return await f();
    }

    public static async ValueTask Bind(this ValueTask task, Func<ValueTask> f)
    {
        await task;
        await f();
        return;
    }

    public static async Task Bind(this Task task, Func<Task> f)
    {
        await task;
        await f();
        return;
    }

    public static async ValueTask<Result<T>> MapAsync<T>(this ValueTask<Result<T>> task)
    {
        var asyncResult = await task;

        return task.AsTask().Status switch
        {
            TaskStatus.RanToCompletion => asyncResult,
            TaskStatus.Faulted => Result<T>.Return(task.AsTask().Exception ?? new Exception("The task did not complete successfully.")),
            TaskStatus.Canceled => Result<T>.Return(new OperationCanceledException("The task was cancelled.")),
            _ => asyncResult
        };
    }

    public static async Task<Result<T>> MapAsync<T>(this Task<Result<T>> task)
    {
        var asyncResult = await task;

        return task.Status switch
        {
            TaskStatus.RanToCompletion => asyncResult,
            TaskStatus.Faulted => Result<T>.Return(task.Exception ?? new Exception("The task did not complete successfully.")),
            TaskStatus.Canceled => Result<T>.Return(new OperationCanceledException("The task was cancelled.")),
            _ => asyncResult
        };
    }
}
