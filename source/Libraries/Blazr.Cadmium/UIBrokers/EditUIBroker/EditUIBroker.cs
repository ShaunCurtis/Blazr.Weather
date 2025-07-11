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

    public async ValueTask<Result> LoadAsync(TKey recordId)
    {
        this.LastResult = await Result<TKey>.Create(recordId)
            // Set the broker state
            .ResultSideEffect(recordId.IsDefault, (id) => this.State = EditState.New, (id) => this.State = EditState.Clean)
            // Check if the broker has already been loaded
            .MapResult<TKey>(id => _isLoaded ? Result<TKey>.Failure("The UIBroker has already been loaded.") : Result<TKey>.Create(id))
            // Get the record item.  This will return a new record if the id is default
            .MapResultAsync<TRecord>(_entityProvider.RecordRequestAsync)
            // Set up the EditMutator and EditContext
            .TaskSideEffectAsync<TRecord>(success: record =>
            {
                this.EditMutator = new();
                this.EditMutator.Load(record!);
                this.EditContext = new EditContext(EditMutator);
                _isLoaded = true;
            })
            .MapTaskAsync();

        return this.LastResult;
    }

    public ValueTask ResetItemAsync()
    {
        var result = _isLoaded.SideEffect(
            isTrue: () =>
            {
                EditMutator.Reset();
                // Create a new EditContext - will reset and rebuild the whole Edit Form
                this.EditContext = new EditContext(EditMutator);
            });

        return ValueTask.CompletedTask;
    }

    public async ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        this.LastResult = await this.UpdateRecordAsync(refreshOnNew);
    }

    public ValueTask DeleteItemAsync()
    {
        var result = _isLoaded.SideEffect(
          isTrue: async () =>
          {
              this.State = EditState.Deleted;
              this.LastResult = await this.UpdateRecordAsync();
          });

        return ValueTask.CompletedTask;
    }

    private async Task<Result> UpdateRecordAsync(bool refreshOnNew = true)
    {
        var result = await EditMutator.ToRecord
             // Set the broker state to dirty
             .ResultSideEffect((value) => this.State = this.State.AsDirty)
             // Save the record item to the datastore
             .MapResultAsync<TKey>((record) => _entityProvider.RecordCommandAsync(StateRecord<TRecord>.Create(record, this.State)))
             // Set the broker state to clean
             .TaskSideEffectAsync((id) => _isLoaded = false);

             // If the record is new, we want to refresh the broker with the new record
        return await result.MapResultAsync(     
            test: refreshOnNew,
            isTrue: async (id) => await LoadAsync(id));
    }
}