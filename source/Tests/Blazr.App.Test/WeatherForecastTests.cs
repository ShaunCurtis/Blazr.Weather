/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Blazr.App.Core;
using Blazr.App.Presentation;
using Blazr.Cadmium;
using Blazr.Cadmium.Core;
using Blazr.Cadmium.QuickGrid;
using Blazr.Diode;
using Blazr.Diode.Mediator;
using Blazr.Gallium;
using Blazr.Manganese;
using Microsoft.Extensions.DependencyInjection;

namespace Blazr.Test;

public partial class WeatherForecastTests
{

    [Fact]
    public async Task GetAForecast()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        //Injects the data broker
        var broker = provider.GetService<IMediatorBroker>()!;
        var messageBus = provider.GetService<IMessageBus>()!;
        var entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;

        // Get the test item and it's Id from the Test Provider
        var testItem = _testDataProvider.WeatherForecasts.First();

        var testRecord = this.AsDmoWeatherForecast(testItem);

        var testId = new WeatherForecastId(testItem.WeatherForecastID);

        //Outputs from the process that need to be tested
        bool result = false;
        DmoWeatherForecast? dbRecord = null;

        var recordResult = await entityProvider.RecordRequest(testId)
            .SideEffectAsync(
            success: (record) =>
            {
                dbRecord = record;
                result = true;
            });

        // check the query was successful
        Assert.True(result);
        // check it matches the test record
        Assert.Equal(testRecord, dbRecord);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(0, 50)]
    [InlineData(5, 10)]
    public async Task GetForecastList(int startIndex, int pageSize)
    {
        var provider = GetServiceProvider();

        //Injects the data broker
        var entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;

        // Get the total expected count and the first record of the page
        var testCount = _testDataProvider.WeatherForecasts.Count();
        var testFirstItem = _testDataProvider.WeatherForecasts.Skip(startIndex).First();

        var testFirstRecord = this.AsDmoWeatherForecast(testFirstItem);

        //Outputs from the process that need to be tested
        bool result = false;
        ListItemsProvider<DmoWeatherForecast> listItemsProvider = default!;

        await GridState<DmoWeatherForecast>
            .Create(pageSize: pageSize, startIndex: startIndex)
            .MapToResultAsync(entityProvider.ListRequest)
            .MatchAsync(
                success: (provider) =>
                {
                    listItemsProvider = provider;
                    result = true;
                });

        Assert.True(result);
        Assert.Equal(testCount, listItemsProvider.TotalCount);
        Assert.Equal(pageSize, listItemsProvider.Items.Count());
        Assert.Equal(testFirstRecord, listItemsProvider.Items.First());
    }

    [Fact]
    public async Task GetAFilteredForecastList()
    {
        var provider = GetServiceProvider();

        //Injects the data broker
        var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
        var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

        // Set up the test data
        var pageSize = 2;
        var testSummary = "Warm";
        var testQuery = _testDataProvider.WeatherForecasts.Where(item => testSummary.Equals(item.Summary, StringComparison.CurrentCultureIgnoreCase));
        var testCount = testQuery.Count();
        var testFirstItem = this.AsDmoWeatherForecast( testQuery.First());

        //Outputs from the process that need to be tested
        bool result = false;
        ListItemsProvider<DmoWeatherForecast> listItemsProvider = default!;

        // We create a Result from a new WeatherForecastListRequest defining our test parameters
        // and then map it to the WeatherListRequest method of the entity provider
        // This will execute the request and return a ListItemsProvider<DmoWeatherForecast> Result
        // which we then match to get the items provider.

        await Result<WeatherForecastListRequest>
            .Create(new()
            {
                PageSize = pageSize,
                StartIndex = 0,
                Summary = testSummary
            })
            .MapAsync<ListItemsProvider<DmoWeatherForecast>>(entityProvider.WeatherListRequest)
            .MatchAsync(success: (provider) =>
            {
                listItemsProvider = provider;
                result = true;
            });

        Assert.True(result);

        // Test the results are as expected
        Assert.Equal(testCount, listItemsProvider.TotalCount);
        Assert.Equal(pageSize, listItemsProvider.Items.Count());
        Assert.Equal(testFirstItem, listItemsProvider.Items.First());
    }

    //[Fact]
    //public async void GetASortedForecastList()
    //{
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    var testCount = _testDataProvider.WeatherForecasts.Count();
    //    var testFirstItem = _testDataProvider.WeatherForecasts.Last();

    //    SortDefinition sort = new("Date", true);
    //    var sortList = new List<SortDefinition>() { sort }; 

    //    var request = new ListQueryRequest { PageSize = 10000, StartIndex = 0, Sorters = sortList };
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    Assert.Equal(testFirstItem, loadResult.Items.First());

    //    sort = new("Date", false);
    //    sortList = new List<SortDefinition>() { sort };

    //    request = new ListQueryRequest { PageSize = 100000, StartIndex = 0, Sorters = sortList };
    //    loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    Assert.Equal(testFirstItem, loadResult.Items.Last());
    //}

    //[Fact]
    //public async void UpdateAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // Get a record id to edit
    //    var testItem = _testDataProvider.WeatherForecasts.First();
    //    var testUid = testItem.WeatherForecastUid;

    //    // Build an item query and execute it against the broker to get the record to edit
    //    var request = ItemQueryRequest.Create(testUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);
    //    var dbItem = loadResult.Item!;

    //    // construct a recordEditContext for the record
    //    // Normally you would plug your edit form fields into this context
    //    // We just update the temperature
    //    var recordEditContext = new WeatherForecastEditContext(dbItem);
    //    recordEditContext.TemperatureC = recordEditContext.TemperatureC + 10;

    //    // In a real edit setting, you would be doing validation to ensure the
    //    // recordEditContext values are valid before attempting to save the record
    //    // Note that the validation is on the WeatherForecastEditContext, not WeatherForecast!
    //    var newItem = recordEditContext.AsRecord;

    //    // Create an update command and execute it against the broker
    //    var command = new CommandRequest<WeatherForecast>(newItem, CommandState.Update);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // Get the updated record from the broker and test they are the same
    //    request = ItemQueryRequest.Create(testUid);
    //    loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);
    //    var dbNewItem = loadResult.Item!;
    //    Assert.Equal(newItem, dbNewItem);

    //    // Execute a list query against the data broker and check the count is still the same
    //    // i.e. we haven't added a record instead of updating one
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);

    //    var testCount = _testDataProvider.WeatherForecasts.Count();
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}

    //[Fact]
    //public async void DeleteAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    // get the test record
    //    var testItem = _testDataProvider.WeatherForecasts.First();
    //    var testUid = testItem.WeatherForecastUid;
    //    var testCount = _testDataProvider.WeatherForecasts.Count() - 1;

    //    // build a command and execute it against the database
    //    var command = new CommandRequest<WeatherForecast>(testItem, CommandState.Delete);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // build a item request and ensure the record no longwer exists
    //    var request = ItemQueryRequest.Create(testUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.False(loadResult.Successful);

    //    // build a list query and check we have one less rcord 
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}

    //[Fact]
    //public async void AddAForecast()
    //{
    //    // Get a fully stocked DI container
    //    var provider = GetServiceProvider();
    //    var broker = provider.GetService<IDataBroker>()!;

    //    var testCount = _testDataProvider.WeatherForecasts.Count() + 1;

    //    // Create a new record
    //    var newItem = new WeatherForecast { WeatherForecastUid = Guid.NewGuid(), Date = DateOnly.FromDateTime(DateTime.Now), Summary = "Testing", TemperatureC = 30 };

    //    // Create a command and execute it against the broker
    //    var command = new CommandRequest<WeatherForecast>(newItem, CommandState.Add);
    //    var commandResult = await broker.ExecuteCommandAsync<WeatherForecast>(command);
    //    Assert.True(commandResult.Successful);

    //    // Create a item query, execute it against the broker and check the new record exists
    //    var request = ItemQueryRequest.Create(newItem.WeatherForecastUid);
    //    var loadResult = await broker.ExecuteQueryAsync<WeatherForecast>(request);
    //    Assert.True(loadResult.Successful);

    //    var dbNewItem = loadResult.Item!;
    //    Assert.Equal(newItem, dbNewItem);

    //    // create a list query and check thr total count has increased by 1 
    //    var queryRequest = new ListQueryRequest { PageSize = 10, StartIndex = 0 };
    //    var queryResult = await broker.ExecuteQueryAsync<WeatherForecast>(queryRequest);
    //    Assert.True(queryResult.Successful);
    //    Assert.Equal(testCount, queryResult.TotalCount);
    //}
}
