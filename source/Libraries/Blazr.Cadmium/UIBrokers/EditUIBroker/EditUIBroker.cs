/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

namespace Blazr.Cadmium.Presentation;

public class EditUIBroker<TRecord, TRecordEditContext, TKey> : IEditUIBroker<TRecordEditContext, TKey>
    where TRecord : class, new()
    where TRecordEditContext : IRecordEditContext<TRecord>, new()
    where TKey : notnull, IEntityId
{
    private readonly IEntityProvider<TRecord, TKey> _entityProvider;

    protected TKey EntityId = default!;
    private bool _isLoaded;
    public EditState State { get; private set; } = EditState.Clean;

    public Result LastResult { get; protected set; } = Result.Success();

    public TRecordEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditUIBroker(IEntityProvider<TRecord, TKey> entityProvider)
    {
        _entityProvider = entityProvider;

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(TKey? id)
    {
        if (_isLoaded)
        {
            LastResult = Result.Failure("The UIBroker has already been loaded. You cannot reload the UIBroker.");
            return;
        }

        // check if we have a real Id to get
        if (id is TKey key && !key.IsDefault)
        {
            this.EntityId = key;
            await GetRecordItemAsync();
            return;
        }

        // We don't have a real Id, so we need to initialize with a new item
        await this.GetNewItemAsync();
    }

    public ValueTask ResetItemAsync()
    {
        if (!_isLoaded)
            return ValueTask.CompletedTask;

        EditMutator.Reset();

        // Create a new EditContext.
        // This will reset and rebuild the whole Edit Form
        this.EditContext = new EditContext(EditMutator);

        return ValueTask.CompletedTask;
    }

    public ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        Debug.Assert(_isLoaded);

        return this.UpdateRecordAsync(refreshOnNew);
    }

    public async ValueTask DeleteItemAsync()
    {
        Debug.Assert(_isLoaded);

        this.State = EditState.Deleted;
        await this.UpdateRecordAsync();
    }

    private ValueTask GetNewItemAsync()
    {
        this.LastResult = Result.Success();

        var record = _entityProvider.NewRecord;

        this.EditMutator = new();
        this.EditMutator.Load(record);

        this.EditContext = new EditContext(EditMutator);

        this.State = EditState.New;
        _isLoaded = true;

        return ValueTask.CompletedTask;
    }

    private async ValueTask GetRecordItemAsync()
    {
        this.LastResult = Result.Success();

        var asyncResult = await _entityProvider.RecordRequest.Invoke(this.EntityId);

        LastResult = asyncResult.Map();

        asyncResult.Output(
            success: record =>
            {
                this.EditMutator = new();
                this.EditMutator.Load(record!);

                this.EditContext = new EditContext(EditMutator);
            });

        _isLoaded = true;
    }

    private async ValueTask UpdateRecordAsync(bool refreshOnNew = true)
    {
        LastResult = Result.Failure("Nothing to Do");

        // Update the command state for an update operation
        if (this.State == EditState.Clean)
            this.State = this.EditMutator.IsDirty ? EditState.Dirty : this.State;

        var mutatedResult = EditMutator.AsRecord;

        var commandResult = await _entityProvider.RecordCommand.Invoke(mutatedResult, this.State);

        this.LastResult = commandResult.Map();

        //TODO - Not sure this will work!!!
        var asyncResult = commandResult.Map<ValueTask>(
            success: key =>
            {
                this.EntityId = _entityProvider.GetKey(key);
                var task = GetRecordItemAsync();
                return Result<ValueTask>.Create(task);
            },
            failure: error => Result<ValueTask>.Create(ValueTask.CompletedTask)
            );

        asyncResult.Output(async task => await task);
        
    }
}