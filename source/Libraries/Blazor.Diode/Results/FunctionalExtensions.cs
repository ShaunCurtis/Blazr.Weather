/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Threading.Tasks;

namespace Blazr.Diode;

public static class FunctionalExtensions
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

    public static async ValueTask<Result<T,U>> Bind<U>(Func<T, U> func)
    {
        if (_exception is not null)
            return Result<U>.Return(_exception!);

        try
        {
            return Result<U>.Return(func(_value!));
        }
        catch (Exception ex)
        {
            return Result<U>.Return(ex);
        }
    }
}
