/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Presentation;

public class WeatherForecastEditPresenterFactory : IEditPresenterFactory<WeatherForecastEditContext, WeatherForecastId>
{
    private IServiceProvider _serviceProvider;
    public WeatherForecastEditPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IEditPresenter<WeatherForecastEditContext, WeatherForecastId>> GetPresenterAsync(WeatherForecastId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<WeatherForecastEditPresenter>(_serviceProvider);
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        await presenter.LoadAsync(id);

        return presenter;
    }
}


public class WeatherForecastReadPresenterFactory : IReadPresenterFactory<DmoWeatherForecast, WeatherForecastId>
{
    private IServiceProvider _serviceProvider;
    public WeatherForecastReadPresenterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async ValueTask<IReadPresenter<DmoWeatherForecast, WeatherForecastId>> GetPresenterAsync(WeatherForecastId id)
    {
        var presenter = ActivatorUtilities.CreateInstance<WeatherForecastReadPresenter>(_serviceProvider);
        ArgumentNullException.ThrowIfNull(presenter, nameof(presenter));
        await presenter.LoadAsync(id);

        return presenter;
    }
}
