/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.Diode;

public sealed class EntityState<T>
{
    private StateRecord<T>? _lastState;

    public EditState State { get; private set; }
        = EditState.Clean;

    public T Record { get; private set; }

    public bool IsDirty
        => this.State != EditState.Clean;

    public StateRecord<T> AsRecord
        => new(this.Record, this.State);

    public EntityState(T item, bool isNew = false)
    {
        this.Record = item;
        this.State = isNew
            ? EditState.New
            : EditState.Clean;
    }

    public Result Update(T record, Guid transactionId)
    {
        this.SaveState(transactionId);

        this.Record = record;
        this.State = this.State.AsDirty;
        return Result.Return();
    }

    public Result MarkAsDeleted(Guid transactionId)
    {
        this.SaveState(transactionId);

        this.State = EditState.Deleted;
        return Result.Return();
    }

    private void SaveState(Guid? transactionId = null)
        => _lastState = new(this.Record, this.State, transactionId ?? Guid.NewGuid());


    public Result RollBackLastUpdate(Guid transactionId)
    {
        if (this._lastState is null)
            return Result.ReturnException("No rollback state available.");

        if (_lastState?.TransactionId != transactionId)
            return Result.ReturnException("There is no rollback data for the transaction.");


        this.Record = _lastState.Record;
        this.State = _lastState.State;
        _lastState = null;

        return Result.Return();
    }
}

