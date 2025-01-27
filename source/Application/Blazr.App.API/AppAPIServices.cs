/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

using Microsoft.AspNetCore.Builder;

namespace Blazr.App.API;

public static class AppAPIServices
{
    public static void AddAppAPIEndpoints(this WebApplication app)
    {
        app.AddWeatherForecastApiEndpoints();
    }
}
