/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public static class TaskFunctionalExtensions
{
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
