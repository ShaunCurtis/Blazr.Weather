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

public partial class WeatherForecastEntityEditUIBroker
{
    public WeatherForecastId Id => _entity?.Id ?? WeatherForecastId.Default;

    public Result LastResult { get; protected set; } = Result.Success();

    public WeatherForecastEditContext EditMutator { get; protected set; } = new();

    public EditContext EditContext { get; protected set; }

    public WeatherForecastEntityEditUIBroker(IEntityProvider<DmoWeatherForecast, WeatherForecastId> entityProvider)
    {
        _entityProvider = entityProvider as WeatherForecastEntityProvider ?? throw new Exception("The provided EntityProvider is not a WeatherForecastEntityProvider ");

        this.EditContext = new EditContext(EditMutator);
    }

    public async ValueTask LoadAsync(WeatherForecastId id)
    {
        LastResult = await Result<WeatherForecastId>.Create(id)
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: id => Task.FromResult(LoadedResult),
                isFalse: id => this.LoadEntityAsync(id));
    }

    public ValueTask ResetItemAsync()
    {
        LastResult = Result.Success()
            .MapToResult(
                test: _isLoaded,
                isTrue: () => Result.Success(),
                isFalse: () => NotLoadedResult
            )
            .SideEffect(
            success: () =>
                {
                    // Reset the EditMutator
                    EditMutator.Reset();
                    // Create a new EditContext.
                    // This will reset and rebuild the whole Edit Form
                    this.EditContext = new EditContext(EditMutator);
                }
            );

        return ValueTask.CompletedTask;
    }

    public async ValueTask SaveItemAsync(bool refreshOnNew = true)
    {
        LastResult = await Result.Success()
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: () => this.UpdateEntityAsync(refreshOnNew),
                isFalse: () => Task.FromResult(NotLoadedResult));
    }

    public async ValueTask DeleteItemAsync()
    {
        LastResult = await Result.Success()
            .MapToResultAsync(
                test: _isLoaded,
                isTrue: () => this.DeleteEntityAsync(),
                isFalse: () => Task.FromResult(NotLoadedResult));
    }
}

public partial class WeatherForecastEntityEditUIBroker
{
    private readonly WeatherForecastEntityProvider _entityProvider;
    private WeatherForecastEntity _entity = default!;
    private bool _isLoaded;

    private static Result NotLoadedResult
        => Result.Failure("The UIBroker has not been loaded. There is nothing to save.");

    private static Result LoadedResult
        => Result.Failure("The UIBroker has already been loaded. You can not reload it.");

    private async Task<Result> LoadEntityAsync(WeatherForecastId id)
        => await Result<WeatherForecastId>.Create(id)
            .MapToResultAsync<WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .TaskSideEffectAsync(
                success: (entity) =>
                {
                    _entity = entity;
                    this.EditMutator = new();
                    this.EditMutator.Load(entity.WeatherForecast);
                    this.EditContext = new EditContext(EditMutator);
                    _isLoaded = true;
                })
            .MapTaskToResultAsync();

    private async Task<Result> UpdateEntityAsync(bool refreshOnNew = true)
    { 
        var result = await WeatherForecastEntity.UpdateWeatherForecastAction
            .CreateAction(EditMutator.AsRecord)
            .AddSender(this)
            .ExecuteAction(_entity)
            .MapToResultAsync(_entityProvider.EntityCommandAsync)
            .MapTaskAsync<WeatherForecastId, WeatherForecastEntity>(_entityProvider.EntityRequestAsync)
            .MapTaskToResultAsync();
    
        var x = WeatherForecastEntity.MarkAsPersistedAction
            .CreateAction()
            .AddSender(this)
            .ExecuteAction(_entity)
            .MapToResultAsync(_entityProvider.EntityCommandAsync)
            .MapTaskToResultAsync();
    }
    private async Task<Result> DeleteEntityAsync()
        => await WeatherForecastEntity.DeleteWeatherForecastAction
            .CreateAction()
            .AddSender(this)
            .ExecuteAction(_entity)
            .MapToResultAsync(_entityProvider.EntityCommandAsync)
            .MapTaskToResultAsync();

}
