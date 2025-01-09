var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Blazr_Weather_Wasm_Server>("blazr-weather-wasm-server");

builder.AddProject<Projects.Blazr_Weather_Server>("blazr-weather-server");

builder.Build().Run();
