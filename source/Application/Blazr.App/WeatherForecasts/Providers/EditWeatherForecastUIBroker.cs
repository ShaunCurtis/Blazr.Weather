/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Cadmium.Core;
using Blazr.Diode;
using Microsoft.AspNetCore.Components.Forms;
using System.Diagnostics;

namespace Blazr.Cadmium.Presentation;

public class EditWeatherForecastUIBroker
{
    private readonly WeatherForecastEntityProvider _entityProvider;
    private WeatherForecastEntity _entity = default!;
    private YesNo _isLoaded;

    public EditState State { get; private set; } = EditState.Clean;

    public Result LastResult { get; protected set; } = Result.Return();

    public WeatherForecastEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditWeatherForecastUIBroker(IEntityProvider<DmoWeatherForecast, WeatherForecastId> entityProvider)
    {
        _entityProvider = entityProvider as WeatherForecastEntityProvider ?? throw new Exception("The provided EntityProvider is not a WeatherForecastEntityProvider ");

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(WeatherForecastId id)
    {
        _isLoaded.Match(
            yes: () =>
            {
                LastResult = Result.ReturnException("The UIBroker has already been loaded. You cannot reload the UIBroker.");
            },
            no: () =>
            {
                LastResult = Result.Return();
            }
        );

        // check if we have a real Id to get
        if (id.IsDefault)
        {
            await GetRecordItemAsync(id);
            return;
        }

        // We don't have a real Id, so we need to initialize with a new item
        await this.GetNewItemAsync();
    }

    private async ValueTask<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var broker = this;

        return await _entityProvider.EntityRequest(id)
            .Match(
                success: entity =>
                {
                    _entity = entity;
                    broker.EditMutator = new();
                    broker.EditMutator.Load(entity.WeatherForecast);

                    broker.EditContext = new EditContext(EditMutator);
                },
                failure: ex => broker.LastResult = Result.Return(ex)
            );
    }

    private Result<WeatherForecastEntity> GetNewEntity()
    {
        this.LastResult = Result.Return();

        _entity = _entityProvider.NewEntity;

        this.EditMutator = new();
        this.EditMutator.Load(_entity.WeatherForecast);

        this.EditContext = new EditContext(EditMutator);

        this.State = EditState.New;
        _isLoaded = true;

        return ValueTask.CompletedTask;
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
        this.LastResult = Result.Return();

        _entity = _entityProvider.NewEntity;

        this.EditMutator = new();
        this.EditMutator.Load(_entity.WeatherForecast);

        this.EditContext = new EditContext(EditMutator);

        this.State = EditState.New;
        _isLoaded = true;

        return ValueTask.CompletedTask;
    }

    private async ValueTask GetRecordItemAsync(WeatherForecastId id)
    {
        this.LastResult = Result.Return();

        var asyncResult = await _entityProvider.EntityRequest(id);

        LastResult = asyncResult.MapToResult();

        asyncResult.MatchSuccess(
            success: entity =>
            {
                _entity = entity;
                this.EditMutator = new();
                this.EditMutator.Load(entity.WeatherForecast);

                this.EditContext = new EditContext(EditMutator);
            });

        _isLoaded = true;
    }

    private async ValueTask UpdateRecordAsync(bool refreshOnNew = true)
    {
        LastResult = Result.ReturnException("Nothing to Do");

        // Update the command state for an update operation
        if (this.State == EditState.Clean)
            this.State = this.EditMutator.IsDirty ? EditState.Dirty : this.State;

        var mutatedRecord = EditMutator.AsRecord;

        var entityResult = WeatherForecastEntity.UpdateWeatherForecastAction.Create(mutatedRecord).WithTransactionId(Guid.NewGuid())
            .WithSender(this)
            .Execute(_entity);

            //.Match(
            //    success: () =>
            //    {
            //        this.State = EditState.Clean;
            //        LastResult = Result.Return();
            //        if (refreshOnNew && this.State == EditState.New)
            //            _ = GetRecordItemAsync(_entity.Id);
            //    },
            //    failure: ex =>
            //    {
            //        LastResult = Result.Return(ex);
            //    }
            );




        var commandResult = await _entityProvider.EntityCommand(_entity);

        this.LastResult = commandResult.MapToResult();

        //TODO - Not sure this will work!!!
        var asyncResult = commandResult.Map<ValueTask>(
            success: key =>
            {
                this.EntityId = _entityProvider.GetKey(key);
                var task = GetRecordItemAsync();
                return Result<ValueTask>.Return(task);
            },
            failure: error => Result<ValueTask>.Return(ValueTask.CompletedTask)
            );

        asyncResult.MatchSuccess(async task => await task);

    }
}