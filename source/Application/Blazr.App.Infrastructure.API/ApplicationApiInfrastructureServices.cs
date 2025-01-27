/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

public static class ApplicationApiInfrastructureServices
{
    public static void AddAppApiInfrastructureServices(this IServiceCollection services)
    {
        var assemblies = new[] { 
            typeof(DmoWeatherForecast).Assembly,
            typeof(DboWeatherForecast).Assembly,
            typeof(ApplicationApiInfrastructureServices).Assembly 
        };

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(assemblies)
        );

        services.AddScoped<IMessageBus, MessageBus>();

        // Add any individual entity services
        services.AddWeatherForecastServerInfrastructureServices();
    }
}
