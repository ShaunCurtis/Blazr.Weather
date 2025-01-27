/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
namespace Blazr.App.Infrastructure.Server;

public static class ApplicationServerInfrastructureServices
{
    public static void AddAppServerInfrastructureServices(this IServiceCollection services)
    {
        services.AddDbContextFactory<InMemoryTestDbContext>(options
            => options.UseInMemoryDatabase($"TestDatabase-{Guid.NewGuid().ToString()}"));

        var assemblies = new[] {
            typeof(DmoWeatherForecast).Assembly,
            typeof(DboWeatherForecast).Assembly,
            typeof(ApplicationServerInfrastructureServices).Assembly
        };

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(assemblies)
        );

        services.AddScoped<IMessageBus, MessageBus>();

        // Add the standard handlers
        services.AddScoped<IListRequestHandler, ListRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<IItemRequestHandler, ItemRequestServerHandler<InMemoryTestDbContext>>();
        services.AddScoped<ICommandHandler, CommandServerHandler<InMemoryTestDbContext>>();

        // Add any individual entity services
        services.AddWeatherForecastServerInfrastructureServices();
    }

    public static void AddTestData(IServiceProvider provider)
    {
        var factory = provider.GetService<IDbContextFactory<InMemoryTestDbContext>>();

        if (factory is not null)
            TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);
    }
}
