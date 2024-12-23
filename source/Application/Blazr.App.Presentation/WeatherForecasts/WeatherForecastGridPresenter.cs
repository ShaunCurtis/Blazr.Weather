/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class WeatherForecastGridPresenter : GridPresenter<DmoWeatherForecast>
{
    public WeatherForecastGridPresenter(
        IMediator mediator, 
        IMessageBus messageBus, 
        KeyedFluxGateStore<GridState, Guid> keyedFluxGateStore)
        : base(mediator, messageBus, keyedFluxGateStore)
    { }

    protected override async Task<ListQueryResult<DmoWeatherForecast>> GetItemsAsync(GridState state)
    {
        // Get the list request from the Flux Context and get the result
        var listRequest = new WeatherForecastListRequest()
        {
            PageSize = state.PageSize,
            StartIndex = state.StartIndex,
            SortColumn = state.Sorter?.SortField,
            SortDescending = state.Sorter?.SortDescending ?? false
        };

        var result = await _dataBroker.Send(listRequest);

        return result;
    }
}
