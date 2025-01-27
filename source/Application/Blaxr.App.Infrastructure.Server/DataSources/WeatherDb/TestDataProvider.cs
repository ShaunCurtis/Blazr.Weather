﻿/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

/// <summary>
/// A class to build a fixed data set for testing
/// </summary>
public sealed class TestDataProvider
{
    public IEnumerable<DboWeatherForecast> DboWeatherForecasts => _dboWeatherForecasts ?? Enumerable.Empty<DboWeatherForecast>();


    private List<DboWeatherForecast>? _dboWeatherForecasts;

    private TestDataProvider()
        => this.Load();

    public void LoadDbContext<TDbContext>(IDbContextFactory<TDbContext> factory) where TDbContext : DbContext
    {
        using var dbContext = factory.CreateDbContext();

        var dboWeatherForecasts = dbContext.Set<DboWeatherForecast>();

        // Check if we already have a full data set
        if (dboWeatherForecasts.Count() == 0)
            dbContext.AddRange(this.DboWeatherForecasts);

        dbContext.SaveChanges();
    }

    public void Load()
    {
        LoadWeatherForcasts();
    }

    private int _recordsToGet = 1000;

    public static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private void LoadWeatherForcasts()
    {
        var rng = new Random();
        _dboWeatherForecasts = Enumerable.Range(1, _recordsToGet).Select(index => new DboWeatherForecast
        {
            ID = Guid.CreateVersion7(),
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        }).ToList();
    }

    private static TestDataProvider? _provider;

    public static TestDataProvider Instance()
    {
        if (_provider is null)
            _provider = new TestDataProvider();

        return _provider;
    }
}
