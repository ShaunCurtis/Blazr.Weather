/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Antimony.Core;

public readonly record struct CommandResult : IDataResult
{
    public bool Successful { get; private init; }
    public object? KeyValue { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;

    public CommandResult() { }

    public TResult Map<TResult>(Func<object, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return Successful ? onSuccess(this.KeyValue!) : onFailure(this.Message!);
    }

    public TResult Map<TResult>(Func<object, TResult> onSuccess, Func<Exception, TResult> onFailure)
    {
        return Successful ? onSuccess(this.KeyValue!) : onFailure(this.Exception!);
    }

    public static CommandResult Success()
    {
        return new CommandResult { Successful = true, KeyValue = new() };
    }

    public static CommandResult SuccessWithKey(object keyValue)
    {
        return new CommandResult { Successful = true, KeyValue = keyValue };
    }

    public static CommandResult Failure(string message)
    {
        return new CommandResult { Exception = new CommandException(message) };
    }

    public static CommandResult Failure(Exception exception)
    {
        return new CommandResult { Exception = exception };
    }
}

public readonly record struct CommandResult<TKeyValue> : IDataResult
{
    public bool Successful { get; private init; } = false;
    public TKeyValue? KeyValue { get; private init; }
    public Exception? Exception { get; private init; } = new Exception("This Result instance has not been Initialized.");

    public string? Message => this.Exception?.Message ?? null;

    public CommandResult() { }

    public TResult Map<TResult>(Func<object, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return Successful ? onSuccess(this.KeyValue!) : onFailure(this.Message!);
    }

    public TResult Map<TResult>(Func<object, TResult> onSuccess, Func<Exception, TResult> onFailure)
    {
        return Successful ? onSuccess(this.KeyValue!) : onFailure(this.Exception!);
    }

    public static CommandResult<TKeyValue> Success()
    {
        return new CommandResult<TKeyValue> { Successful = true, KeyValue = default };
    }

    public static CommandResult<TKeyValue> SuccessWithKey(TKeyValue keyValue)
    {
        return new CommandResult<TKeyValue> { Successful = true, KeyValue = keyValue };
    }

    public static CommandResult<TKeyValue> Failure(string message)
    {
        return new CommandResult<TKeyValue> { Exception = new CommandException(message) };
    }

    public static CommandResult<TKeyValue> Failure(Exception exception)
    {
        return new CommandResult<TKeyValue> { Exception = exception };
    }
}

