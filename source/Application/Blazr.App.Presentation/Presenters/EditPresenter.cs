/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

namespace Blazr.App.Presentation;

public abstract class EditPresenter<TRecord, TRecordEditContext, TKey> : IEditPresenter<TRecordEditContext, TKey>
    where TRecord : class, new()
    where TRecordEditContext : IRecordEditContext<TRecord>, new()
    where TKey : notnull, IEntityId
{
    private readonly IRecordIdProvider<TKey> _recordIdProvider;
    protected readonly IMediator Databroker;
    private readonly INewRecordProvider<TRecord> _newRecordProvider;

    protected TKey EntityId = default!;
    private bool _isLoaded;
    public CommandState CommandState { get; private set; } = CommandState.None;

    public IDataResult LastResult { get; protected set; } = CommandResult.Success();

    public TRecordEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditPresenter(IMediator mediator, IRecordIdProvider<TKey> keyProvider, INewRecordProvider<TRecord> newRecordProvider)
    {
        this.Databroker = mediator;
        _recordIdProvider = keyProvider;
        _newRecordProvider = newRecordProvider;

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(TKey? id)
    {
        if (_isLoaded)
        {
            LastResult = CommandResult.Failure("The Presenter has already been loaded. You cannot reload the Presenter.");
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
        Debug.Assert(_isLoaded);

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

        this.CommandState = CommandState.Delete;
        await this.UpdateRecordAsync();
    }

    private ValueTask GetNewItemAsync()
    {
        this.LastResult = CommandResult.Success();

        var record = _newRecordProvider.NewRecord();

        this.EditMutator = new();
        this.EditMutator.Load(record);

        this.EditContext = new EditContext(EditMutator);

        this.CommandState = CommandState.Add;
        _isLoaded = true;

        return ValueTask.CompletedTask;
    }

    protected abstract Task<ItemQueryResult<TRecord>> GetItemAsync();

    private async ValueTask GetRecordItemAsync()
    {
        this.LastResult = CommandResult.Success();

        TRecord record;

        var result = await GetItemAsync();
        this.LastResult = result;

        if (!result.Successful)
        {
            this.LastResult = result;
            _isLoaded = true;
            return;
        }

        record = result.Item!;

        this.EditMutator = new();
        this.EditMutator.Load(record);

        this.EditContext = new EditContext(EditMutator);

        _isLoaded = true;
    }
    protected abstract Task<CommandResult<TKey>> UpdateAsync(TRecord record, CommandState state);

    private async ValueTask UpdateRecordAsync(bool refreshOnNew = true)
    {
        LastResult = CommandResult.Failure("Nothing to Do");

        // Update the command state for an update operation
        if (this.CommandState == CommandState.None)
            this.CommandState = this.EditMutator.IsDirty ? CommandState.Update : this.CommandState;

        var mutatedResult = EditMutator.AsRecord;


        var commandResult = await UpdateAsync(mutatedResult, this.CommandState);
        this.LastResult = commandResult;

        if (!commandResult.Successful)
            return;

        if (this.CommandState == CommandState.Add && commandResult.KeyValue is not null && refreshOnNew)
        {
            this.EntityId = _recordIdProvider.GetKey(commandResult.KeyValue);
            await GetRecordItemAsync();
        }
    }


}