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
using Xunit;

namespace Blazr.Test;

public partial class WeatherForecastTests
{

    [Fact]
    public async Task GetAForecast()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        //Injects the data broker
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
            .OutputAsync(
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
        var testFirstItem = this.AsDmoWeatherForecast(testQuery.First());

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
            .OutputAsync(success: (provider) =>
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

    [Fact]
    public async Task GetASortedForecastList()
    {
        var provider = GetServiceProvider();

        //Injects the data broker
        var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
        var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

        // Set up the test data
        var pageSize = 10;
        var testQuery = _testDataProvider.WeatherForecasts.OrderByDescending(item => item.Date);
        var testCount = testQuery.Count();
        var testFirstItem = this.AsDmoWeatherForecast(testQuery.First());
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
                SortColumn = "Date",
                SortDescending = true
            })
            .MapAsync<ListItemsProvider<DmoWeatherForecast>>(entityProvider.WeatherListRequest)
            .OutputAsync(success: (provider) =>
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

    [Fact]
    public async Task UpdateAForecast()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        //Injects the data broker
        var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
        var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

        // Get the test item and it's Id from the Test Provider
        var testItem = _testDataProvider.WeatherForecasts.First();

        var testId = new WeatherForecastId(testItem.WeatherForecastID);

        //Outputs from the process that need to be tested
        bool result = false;
        WeatherForecastEntity entity = default!;
        WeatherForecastId updatedId = default!;

        var recordResult = await entityProvider.EntityRequest(testId)
            .SideEffectAsync(
            success: (item) =>
            {
                entity = item;
                result = true;
            });

        // check the query was successful
        Assert.True(result);

        DmoWeatherForecast testRecord = entity.WeatherForecast;

        var updatedRecord = testRecord with { Summary = "Test Edit" };

        await WeatherForecastEntity.UpdateWeatherForecastAction.Create(updatedRecord)
            .AddSender(this)
            .Execute(entity)
            .MapAsync(entityProvider.EntityCommand)
            .OutputAsync(success: (id) =>
            {
                result = true;
                updatedId = id;
            });

        // check the update was successful
        Assert.True(result);


        result = false;
        DmoWeatherForecast? dbRecord = null;

        await entityProvider.RecordRequest(updatedId)
            .OutputAsync(
            success: (record) =>
            {
                dbRecord = record;
                result = true;
            });

        // check the query was successful
        Assert.True(result);
        // check it matches the update record
        Assert.Equal(updatedRecord, dbRecord);
    }

    [Fact]
    public async Task DeleteAForecast()
    {
        // Get a fully stocked DI container
        var provider = GetServiceProvider();

        //Injects the data broker
        var _entityProvider = provider.GetService<IEntityProvider<DmoWeatherForecast, WeatherForecastId>>()!;
        var entityProvider = (WeatherForecastEntityProvider)_entityProvider;

        // Get the test item and it's Id from the Test Provider
        var testItem = _testDataProvider.WeatherForecasts.First();

        var testId = new WeatherForecastId(testItem.WeatherForecastID);

        //Outputs from the process that need to be tested
        bool result = false;
        WeatherForecastEntity entity = default!;
        WeatherForecastId updatedId = default!;

        var recordResult = await entityProvider.EntityRequest(testId)
            .SideEffectAsync(
            success: (item) =>
            {
                entity = item;
                result = true;
            });

        // check the query was successful
        Assert.True(result);

        DmoWeatherForecast testRecord = entity.WeatherForecast;


        await WeatherForecastEntity.DeleteWeatherForecastAction.Create()
            .AddSender(this)
            .Execute(entity)
            .MapAsync(entityProvider.EntityCommand)
            .OutputAsync(success: (id) =>
            {
                result = true;
                updatedId = id;
            });

        // check the update was successful
        Assert.True(result);

        
        result = false;
        Exception? exception = null;

        await entityProvider.RecordRequest(updatedId)
            .OutputAsync(
            failure: (ex) =>
            {
                exception = ex;
                result = true;
            });

        // check the query was successful
        Assert.True(result);
        // check it matches the update record
        Assert.NotNull(exception);
    }

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
