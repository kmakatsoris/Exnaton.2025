using Exnaton.Implementations;
using Exnaton.Implementations.HttpClients;
using Exnaton.Implementations.Repositories;
using Exnaton.Interfaces;
using Exnaton.Interfaces.HttpClients;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models;
using Exnaton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Exnaton.Utils;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null || configuration == null) 
            throw new ArgumentNullException($"The application is not configured properly.");
        // Singleton,
        services.AddSingleton(Log.Logger);
        
        // Scoped,
        services.AddScoped<IMeasurementsRepository, MeasurementsRepository>();
        services.AddScoped<ITagsRepository, TagsRepository>();
        
        // Transient
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<IAnalysisService, AnalysisService>();
        services.AddTransient<IHttpClientService, HttpClientService>();

        return services;
    }

    public static IServiceCollection ConfigureHttpClients(this IServiceCollection services)
    {
        services.AddHttpClient();
        
        return services;
    }
    
    public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        if (services == null || configuration == null) 
            throw new ArgumentNullException($"The application is not configured properly.");
        // Singleton,
        
        // Scoped,
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(configuration.GetConnectionString("DbConnection"), 
                ServerVersion.AutoDetect(configuration.GetConnectionString("DbConnection"))), ServiceLifetime.Scoped);
        
        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("AppDbContext");
        
        // Transient

        return services;
    }

    public static IServiceCollection ConfigurationSetup(this WebApplicationBuilder builder)
    {
        if (builder == null) 
            throw new ArgumentNullException($"The application is not configured properly.");

        string internalEnv = builder.Configuration.GetSection("InternalEnvironment").Value ?? "Development";
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{internalEnv}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
        builder.Services.Configure<AppSettings>(builder.Configuration);

        builder.ConfigureLogger();

        return builder.Services;
    }

    private static void ConfigureLogger(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration)
            .CreateLogger();
        
        builder.Host.UseSerilog();
    }
    
}