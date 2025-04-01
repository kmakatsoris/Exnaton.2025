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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();