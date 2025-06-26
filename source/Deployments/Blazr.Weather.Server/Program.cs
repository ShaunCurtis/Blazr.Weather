using Blazr.App.Presentation;
using Blazr.App.UI;
using Blazr.Cadmium.Presentation;
using Blazr.Gallium;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Blazr.Diode.Mediator;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var services = builder.Services;

// Add Mediator
services.AddMediator(new Assembly[] {
                typeof(Blazr.App.Weather.EntityFramework.WeatherApplicationServerServices).Assembly
        });

// Add the Gallium Message Bus Server services
services.AddScoped<IMessageBus, MessageBus>();

// InMemory Scoped State Store 
services.AddScoped<ScopedStateProvider>();

// Presenter Factories
services.AddScoped<ILookupUIBrokerFactory, LookupUIBrokerFactory>();
services.AddScoped<IEditUIBrokerFactory, EditUIBrokerFactory>();
services.AddTransient<IReadUIBrokerFactory, ReadUIBrokerFactory>();

// Add the QuickGrid Entity Framework Adapter
services.AddQuickGridEntityFrameworkAdapter();


builder.Services.AddAppServerInfrastructureServices();
builder.Services.AddAppUIServices();
builder.Services.AddAppPresentationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// get the DbContext factory and add the test data
var factory = app.Services.GetService<IDbContextFactory<InMemoryTestDbContext>>();
if (factory is not null)
    TestDataProvider.Instance().LoadDbContext<InMemoryTestDbContext>(factory);

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<Blazr.Weather.Server.Components.App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Blazr.App.UI.ApplicationUIServices).Assembly);

app.Run();
