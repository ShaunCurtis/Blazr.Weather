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

    public async ValueTask<Result<GridItemsProviderResult<DmoWeatherForecast>>> GetItemsAsync(GridState<DmoWeatherForecast> state)
    {
        var asyncResult = await _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

        return asyncResult.Bind<GridItemsProviderResult<DmoWeatherForecast>>(FromListItemsProvider);
    }

    public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> EntityRequest
        => (id) => _mediator.Send(new WeatherForecastRecordRequest(id));


    public Func<WeatherForecastId, Task<Result<DmoWeatherForecast>>> RecordRequest
        => (id) => _mediator.Send(new WeatherForecastRecordRequest(id));

    public Func<DmoWeatherForecast, EditState, Task<Result<WeatherForecastId>>> RecordCommand
        => (record, state) => _mediator.Send(new WeatherForecastCommandRequest(record, state));

    public Func<GridState<DmoWeatherForecast>, Task<Result<ListItemsProvider<DmoWeatherForecast>>>> ListRequest
        => (state) => _mediator.Send(new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.SortField,
            SortDescending = state.SortDescending
        });

    public WeatherForecastEntityProvider(IMediatorBroker mediator, IServiceProvider serviceProvider)
    {
        _mediator = mediator;
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<Result<WeatherForecastEntity>> GetEntityAsync(WeatherForecastId id)
    {
        var result = (await _mediator.Send(new WeatherForecastRecordRequest(id)))
            .MapSuccess<WeatherForecastEntity>((record) =>
            {
                new WeatherForecastEntity(record);
                return Result<WeatherForecastEntity>.ReturnException("");
            });

        return result;
    }

    public WeatherForecastId GetKey(object obj)
    {
        return obj switch
        {
            WeatherForecastId id => id,
            DmoWeatherForecast record => record.Id,
            Guid guid => new WeatherForecastId(guid),
            _ => WeatherForecastId.Default
        };
    }

    public bool TryGetKey(object obj, out WeatherForecastId key)
    {
        key = GetKey(obj);
        return key != WeatherForecastId.Default;
    }

    public DmoWeatherForecast NewRecord
        => new DmoWeatherForecast { Id = WeatherForecastId.Default };
}
