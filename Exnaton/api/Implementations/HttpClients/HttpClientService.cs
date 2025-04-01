using System.Text.Json;
using Exnaton.Exceptions;
using Exnaton.Interfaces.HttpClients;
using Exnaton.Models;
using Microsoft.Extensions.Options;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Implementations.HttpClients;

public class HttpClientService: IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly Serilog.ILogger _logger;
    private readonly AppSettings _appSettings;

    public HttpClientService(HttpClient httpClient,
                             IOptions<AppSettings> appSettings,
                             Serilog.ILogger logger)
    {
        _httpClient = httpClient ?? throw BusinessExceptions.InvalidMeasurementException(_logger,nameof(httpClient));
        _logger = logger;
        _appSettings =  appSettings?.Value ?? throw BusinessExceptions.InvalidMeasurementException(_logger,nameof(appSettings));
    }

    public async Task<MeasurementDataWrapperDTO?> FetchDataAsync(Guid muid)
    {
        if (muid == Guid.Empty)
        {
            throw BusinessExceptions.InvalidMeasurementException(_logger, "MUID cannot be empty.");
        }

        var requestUrl = $"{_appSettings?.ConnectionStrings?.AmazonS3BucketBaseUrl}{muid}.json";
        _logger.Information($"Fetching data from {requestUrl}");

        try
        {
            var response = await _httpClient?.GetAsync(requestUrl);
            response?.EnsureSuccessStatusCode();

            var jsonResponse = await response?.Content?.ReadAsStringAsync();
            return JsonSerializer.Deserialize<MeasurementDataWrapperDTO>(jsonResponse,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (HttpRequestException ex)
        {
            throw BusinessExceptions.ExternalServiceNotAvailableException(_logger,
                logMsg: $"HTTP request error: {ex?.Message}");
        }
        catch (JsonException ex)
        {
            throw BusinessExceptions.ExternalServiceNotAvailableException(_logger,
                logMsg: $"JSON deserialization error: {ex?.Message}");
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ExternalServiceNotAvailableException(_logger,
                logMsg: $"Unhandled exception: {ex?.Message}");
        }
    }
}