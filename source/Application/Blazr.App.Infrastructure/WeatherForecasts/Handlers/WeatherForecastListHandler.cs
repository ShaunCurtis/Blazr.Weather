/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure;

public record WeatherForecastListHandler : IRequestHandler<WeatherForecastListRequest, Result<ListResult<DmoWeatherForecast>>>
{
    private IListRequestHandler listRequestHandler;

    public WeatherForecastListHandler(IListRequestHandler listRequestHandler)
    {
        this.listRequestHandler = listRequestHandler;
    }

    public async Task<Result<ListResult<DmoWeatherForecast>>> Handle(WeatherForecastListRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<DmoWeatherForecast> forecasts = Enumerable.Empty<DmoWeatherForecast>();

        var query = new ListQueryRequest<DboWeatherForecast>()
        {
            PageSize = request.PageSize,
            StartIndex = request.StartIndex,
            SortDescending = request.SortDescending,
            SortExpression = this.GetSorter(request.SortColumn),
            FilterExpression = this.GetFilter(request),
            Cancellation = cancellationToken
        };

        var result = await listRequestHandler.ExecuteAsync<DboWeatherForecast>(query);

        if (!result.HasSucceeded(out ListResult<DboWeatherForecast> listResult))
            return result.ConvertFail<ListResult<DmoWeatherForecast>>();

        var list = listResult.Items.Select(item => DboWeatherForecastMap.Map(item));

        return Result<ListResult<DmoWeatherForecast>>.Success( new(list, listResult.TotalCount));
    }

    private Expression<Func<DboWeatherForecast, object>> GetSorter(string? field)
        => field switch
        {
            "ID" => (Item) => Item.ID,
            "Id" => (Item) => Item.ID,
            "Temperature" => (Item) => Item.TemperatureC,
            "Summary" => (Item) => Item.Summary,
            _ => (item) => item.Date
        };

    private Expression<Func<DboWeatherForecast, bool>>? GetFilter(WeatherForecastListRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Summary))
            return (item) => item.Summary.Equals(request.Summary, StringComparison.CurrentCultureIgnoreCase);

        return null;
    }

}
