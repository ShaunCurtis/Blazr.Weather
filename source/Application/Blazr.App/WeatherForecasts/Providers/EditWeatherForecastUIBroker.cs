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

    public Result LastResult { get; protected set; } = Result.Success();

    public WeatherForecastEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public EditWeatherForecastUIBroker(IEntityProvider<DmoWeatherForecast, WeatherForecastId> entityProvider)
    {
        _entityProvider = entityProvider as WeatherForecastEntityProvider ?? throw new Exception("The provided EntityProvider is not a WeatherForecastEntityProvider ");

        this.EditContext = new EditContext(EditMutator);
    }

    public ValueTask LoadAsync(WeatherForecastId id)
    {
        _isLoaded.Output(
            isTrue: LoadedErrorResult,
            isFalse: async () =>
            {
                LastResult = Result.Success();
                await this.GetEntityAsync(id);
            }
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask ResetItemAsync()
    {
        _isLoaded.Output(
            isTrue: LoadedErrorResult,
            isFalse: () =>
            {
                LastResult = Result.Success();
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
        _isLoaded.Output(
            isFalse: NotLoadedErrorResult,
            isTrue: async () =>
            {
                LastResult = Result.Success();
                await this.UpdateRecordAsync(refreshOnNew);
            }
        );

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteItemAsync()
    {
        _isLoaded.Output(
            isFalse: NotLoadedErrorResult,
            isTrue: async () =>
            {
                LastResult = Result.Success();
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
        => LastResult = Result.Failure("The UIBroker has not been loaded. There is nothing to save.");

    private void LoadedErrorResult()
        => LastResult = Result.Failure("The UIBroker has already been loaded. You can not reload it.");

    private async Task<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var broker = this;

        LastResult = Result.Success();

        return await id.IsDefault
            .MapAsync<WeatherForecastEntity>(
                isTrue: () => _entityProvider.NewEntityAsync,
                isFalse: () => _entityProvider.EntityRequest(id))
            .TaskSideEffectAsync(
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
        LastResult = await WeatherForecastEntity.UpdateWeatherForecastAction
            .CreateAction(EditMutator.AsRecord)
            .AddSender(this)
            .ExecuteAction(_entity)
            .MapResultAsync(_entityProvider.EntityCommand)
            .MapTaskAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequest)
            .MapTaskAsync();
    }

    private async ValueTask DeleteRecordAsync()
    {
        var entityResult = WeatherForecastEntity.DeleteWeatherForecastAction
            .CreateAction()
            .AddSender(this)
            .ExecuteAction(_entity);

        LastResult = await _entityProvider.EntityCommand(_entity)
            .MapTaskAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequest)
            .MapTaskAsync();
    }

}
