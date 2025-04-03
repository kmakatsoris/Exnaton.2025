using Exnaton.Models;
using Exnaton.Utils;
using Microsoft.Extensions.Options;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigurationSetup()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureHttpClients()
    .ConfigureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var serviceProvider = builder.Services.BuildServiceProvider();
var appSettings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;

app.MapGet("/environment", () =>
{
    var environment = builder.Environment.EnvironmentName;
    var version = appSettings.Version;
    return Results.Ok(new { Environment = environment, AppSettingsEnvironment = appSettings?.InternalEnvironment, Version = version, DefaultConnection = appSettings?.ConnectionStrings?.DbConnection ?? "DbConnection not found." });
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment() || appSettings?.InternalEnvironment?.Contains("Development") == true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Exnaton API Endpoints Available");
        c.RoutePrefix = string.Empty; // http://localhost:<Port-web-api ex: 5142 or 8080>/index.html
    });
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();

/*
 * Dependencies:
 * sudo apt install libharfbuzz-dev
 * find ~/.nuget/packages/ -name "libHarfBuzzSharp.so"
 * sudo apt install -y libharfbuzz-dev libfreetype6 libfontconfig1
 * mkdir -p /home/maccos/Workspace/Interviews/Exnaton.2025/Exnaton/bin/Debug/net8.0/linux-x64/
 * cp ~/.nuget/packages/harfbuzzsharp.nativeassets.linux/7.3.0.3/runtimes/linux-x64/native/libHarfBuzzSharp.so    /home/maccos/Workspace/Interviews/Exnaton.2025/Exnaton/bin/Debug/net8.0/linux-x64/
 * export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:/home/maccos/.nuget/packages/harfbuzzsharp.nativeassets.linux/7.3.0.3/runtimes/linux-x64/native/
*/