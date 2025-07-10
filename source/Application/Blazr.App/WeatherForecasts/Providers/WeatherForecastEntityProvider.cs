/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using Blazr.App.Core;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode.Mediator;
using Microsoft.AspNetCore.Components.QuickGrid;

namespace Blazr.App.Presentation;

public class WeatherForecastEntityProvider
   : EntityProvider<DmoWeatherForecast>,
    IEntityProvider<DmoWeatherForecast, WeatherForecastId>
{
    private readonly IMediatorBroker _mediator;
    private readonly IServiceProvider _serviceProvider;

    public async Task<Result<GridItemsProviderResult<DmoWeatherForecast>>> GetItemsAsync(GridState<DmoWeatherForecast> state)
    {
        var asyncResult = await _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

        return asyncResult.MapResult<GridItemsProviderResult<DmoWeatherForecast>>(FromListItemsProvider);
    }

    public Func<WeatherForecastId, Task<Result<WeatherForecastEntity>>> EntityRequest
        => (id) => _mediator.Send(new WeatherForecastEntityRequest(id));

    public Func<WeatherForecastEntity, Task<Result<WeatherForecastId>>> EntityCommand
        => (record) => _mediator.Send(new WeatherForecastEntityCommandRequest(record));

    public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> RecordRequest
        => (id) => _mediator.Send(new WeatherForecastRecordRequest(id));

    public Func<DmoWeatherForecast, EditState, Task<Result<WeatherForecastId>>> RecordCommand
        => (record, state) => _mediator.Send(new WeatherForecastCommandRequest(record, state));

    public Func<GridState<DmoWeatherForecast>, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> GridItemsRequest
        => (state) => _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public Func<WeatherForecastListRequest, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> ListItemsRequest
        => (request) => _mediator.Send(request);

    public WeatherForecastEntityProvider(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var result = (await _mediator.Send(new WeatherForecastRecordRequest(id)))
            .MapResult<WeatherForecastEntity>((record) =>
            {
                new WeatherForecastEntity(record);
                return Result<WeatherForecastEntity>.Failure($"No entity exists for Id{id}.  Created default entity.");
            });

        return result;
    }

    public Result<WeatherForecastId> GetKey(object? obj)
        => obj switch
        {
            WeatherForecastId id => Result<WeatherForecastId>.Create(id),
            DmoWeatherForecast record => Result<WeatherForecastId>.Create(record.Id),
            Guid guid => Result<WeatherForecastId>.Create(new(guid)),
            _ => Result<WeatherForecastId>.Failure($"Could not convert the provided key - {obj?.ToString()}")
        };

    public DmoWeatherForecast NewRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default };

    public Task<Result<WeatherForecastEntity>> NewEntityAsync
        => Task.FromResult(Result<WeatherForecastEntity>.Create(new WeatherForecastEntity(new DmoWeatherForecast())));

    public Result<WeatherForecastEntity> NewEntity
        => Result<WeatherForecastEntity>.Create(new WeatherForecastEntity(new DmoWeatherForecast()));
}
