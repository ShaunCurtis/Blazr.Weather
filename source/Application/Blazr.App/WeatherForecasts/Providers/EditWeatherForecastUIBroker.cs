/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Cadmium.Core;
using Microsoft.AspNetCore.Components.Forms;

namespace Blazr.Cadmium.Presentation;

public partial class EditWeatherForecastUIBroker
{
    public EditState State => _entity.WeatherForecastRecord.State;

    public Result LastResult { get; protected set; } = Result.Return();

    public WeatherForecastEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditWeatherForecastUIBroker(IEntityProvider<DmoWeatherForecast, WeatherForecastId> entityProvider)
    {
        _entityProvider = entityProvider as WeatherForecastEntityProvider ?? throw new Exception("The provided EntityProvider is not a WeatherForecastEntityProvider ");

        this.EditContext = new EditContext(EditMutator);
    }

    public ValueTask LoadAsync(WeatherForecastId id)
    {
        _isLoaded.Match(
            isTrue: LoadedErrorResult,
            isFalse: async () =>
            {
                LastResult = Result.Return();
                await this.GetEntityAsync(id);
            }
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask ResetItemAsync()
    {
        _isLoaded.Match(
            isTrue: LoadedErrorResult,
            isFalse: () =>
            {
                LastResult = Result.Return();
                EditMutator.Reset();

                // Create a new EditContext.
                // This will reset and rebuild the whole Edit Form
                this.EditContext = new EditContext(EditMutator);
            }
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        _isLoaded.Match(
            isFalse: NotLoadedErrorResult,
            isTrue: async () =>
            {
                LastResult = Result.Return();
                await this.UpdateRecordAsync(refreshOnNew);
            }
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteItemAsync()
    {
        _isLoaded.Match(
            isFalse: NotLoadedErrorResult,
            isTrue: async () =>
            {
                LastResult = Result.Return();
                await this.DeleteItemAsync();
            }
        );

        return ValueTask.CompletedTask;
    }
}

// ============================================================

public partial class EditWeatherForecastUIBroker
{
    private readonly WeatherForecastEntityProvider _entityProvider;
    private WeatherForecastEntity _entity = default!;
    private bool _isLoaded;

    private void NotLoadedErrorResult()
        => LastResult = Result.ReturnException("The UIBroker has not been loaded. There is nothing to save.");

    private void LoadedErrorResult()
        => LastResult = Result.ReturnException("The UIBroker has already been loaded. You can not reload it.");

    private async Task<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var broker = this;

        LastResult = Result.Return();

        return await id.IsDefault
            .MapAsync<WeatherForecastEntity>(
                isTrue: () => _entityProvider.NewEntityAsync,
                isFalse: () => _entityProvider.EntityRequest(id))
            .SideEffectAsync(
                success: (entity) =>
                {
                    _entity = entity;
                    broker.EditMutator = new();
                    broker.EditMutator.Load(entity.WeatherForecast);

                    broker.EditContext = new EditContext(EditMutator);

                    _isLoaded = true;
                });
    }

    private async ValueTask UpdateRecordAsync(bool refreshOnNew = true)
    {
        var mutatedRecord = EditMutator.AsRecord;

        var entityResult = WeatherForecastEntity.UpdateWeatherForecastAction
            .Create(mutatedRecord)
            .WithSender(this)
            .Execute(_entity);

        LastResult = await _entityProvider.EntityCommand(_entity)
            .AndThenAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequest)
            .MapToResultAsync();
    }

    private async ValueTask DeleteRecordAsync()
    {
        var entityResult = WeatherForecastEntity.DeleteWeatherForecastAction
            .Create()
            .WithSender(this)
            .Execute(_entity);

        LastResult = await _entityProvider.EntityCommand(_entity)
            .AndThenAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequest)
            .MapToResultAsync();
    }

}
