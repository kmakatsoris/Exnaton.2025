using System.Text.Json;
using Exnaton.Implementations;
using Exnaton.Implementations.HttpClients;
using Exnaton.Implementations.Repositories;
using Exnaton.Interfaces;
using Exnaton.Interfaces.HttpClients;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models;
using Exnaton.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
        Log.Logger.Information("Logger is successfully configured.");
        Log.Logger.Error("Logger is successfully configured.");
        
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
        var appSettings = services.BuildServiceProvider().GetRequiredService<IOptions<AppSettings>>().Value;
        string connectionString = appSettings?.ConnectionStrings?.DbConnection ?? throw new ArgumentNullException("Connection string not found.");
        Log.Logger.Information($"connectionString={JsonSerializer.Serialize(connectionString)}.");
        
        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, 
                ServerVersion.AutoDetect(connectionString)), ServiceLifetime.Scoped);
        
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

        var config = builder.Configuration;
        try
        {
            string host = "Server=" + (Environment.GetEnvironmentVariable("MYSQL_HOST") ??
                                       throw new ArgumentNullException("MYSQL_HOST")) + ";";
            string database = "Database=" + (Environment.GetEnvironmentVariable("MYSQL_DATABASE") ??
                                             throw new ArgumentNullException("MYSQL_DATABASE")) + ";";
            string user = "User=" + (Environment.GetEnvironmentVariable("MYSQL_USER") ??
                                     throw new ArgumentNullException("MYSQL_USER")) + ";";
            string password = "Password=" + (Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ??
                                             throw new ArgumentNullException("MYSQL_PASSWORD")) + ";";
            if (string.IsNullOrEmpty(config.GetConnectionString("DbConnection")))
                throw new ArgumentException("The connection string is missing.");
            config["ConnectionStrings:DbConnection"] = host+database+user+password;
        }
        catch (Exception ex)
        {
            
        }
        
        
        builder.Services.Configure<AppSettings>(config);
        
        

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