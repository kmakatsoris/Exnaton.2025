using System.Text.Json;
using Exnaton.Exceptions;
using Exnaton.Interfaces;
using Exnaton.Interfaces.HttpClients;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models.Entities;
using Exnaton.Models.Requests;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Implementations;

public class DataService: IDataService
{
    private readonly Serilog.ILogger _logger;
    private readonly IMeasurementsRepository _measurementsRepository;
    private readonly IHttpClientService _httpClientService;
    private readonly ITagsRepository _tagsRepository;
    
    public DataService(Serilog.ILogger logger, 
        IMeasurementsRepository measurementsRepository, 
        IHttpClientService httpClientService,
        ITagsRepository  tagsRepository)
    {
        _logger = logger;    
        _measurementsRepository = measurementsRepository;
        _httpClientService = httpClientService;
        _tagsRepository = tagsRepository;
    }

    /// <summary>
    /// Fetches measurement data from an external source based on the provided request
    /// and stores it in the database.
    /// </summary>
    /// <param name="request">The request containing filtering criteria such as MUID, measurement type, time range, and limits.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidMeasurementException">Thrown when the request is invalid.</exception>
    /// <exception cref="ExternalServiceNotAvailableException">Thrown when the response is invalid or when face HttpRequestException or JsonException or other unhandled exception.</exception>
    public async Task<List<MeasurementDataDTO>> FetchAndStoreDataAsync(ReadMeasurementsRequest request, bool enReturnResponse = false)
    {
        _logger.Error("@TEST: Welcome to FetchAndStoreDataAsync");
        if (request?.Validate() != true)
            throw BusinessExceptions.InvalidMeasurementException(_logger, $"Invalid request.", logMsg: $"ReadMeasurementsRequest: {JsonSerializer.Serialize(request)}");
        MeasurementDataWrapperDTO? res = await _httpClientService?.FetchDataAsync(request.Muid);
        if (res == null || res?.Data == null || res?.Data?.Count == 0 || res.Data.Any(x => x == null))
            throw BusinessExceptions.ExternalServiceNotAvailableException(_logger, 
                logMsg: $"Error fetching data for {request.Muid} inside the FetchAndStoreDataAsync function. @Indications: {res == null} || {res?.Data == null} ||{res?.Data?.Count == 0} || {res.Data.Any(x => x == null)}");
        List<MeasurementDataEntity> newRecs = await _measurementsRepository?.AddAsync(res?.Data);
        string qualityValue = await _tagsRepository?.ReadQualityFromTagMUId(newRecs?.FirstOrDefault()?.TagsMUId ?? Guid.Empty);
        return newRecs
            ?.Select(r =>
                {
                    var dto = (MeasurementDataDTO)r;
                    if (dto?.Tags?.Quality != null)
                        dto.Tags.Quality = qualityValue;
                    return dto;
                })
            ?.Where(m => m != null && 
                        m.Measurement == request.Measurement &&
                        m.Timestamp >= request.Start &&
                        m.Timestamp <= request.Stop)
            ?.Take(request?.Limit > 0 ? request.Limit : int.MaxValue)
            ?.ToList() ?? new List<MeasurementDataDTO>();
    }

    /// <summary>
    /// Reads measurement data from the database based on the provided request parameters.
    /// </summary>
    /// <param name="request">The request containing filtering criteria such as measurement type, limit, start date, and stop date.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a list of <see cref="MeasurementDataDTO"/> objects 
    /// or null if no data is found.
    /// </returns>
    /// <exception cref="BusinessException">Thrown when the request contains invalid parameters.</exception>
    public async Task<List<MeasurementDataDTO>?> ReadDataAsync(ReadMeasurementsRequest request)
    {
        _logger.Error("@TEST: Welcome to ReadDataAsync");
        if (request?.Validate() != true)
            throw BusinessExceptions.InvalidMeasurementException(_logger, $"Invalid request.", logMsg: $"ReadMeasurementsRequest: {JsonSerializer.Serialize(request)}");
        List<MeasurementDataDTO>? data = _measurementsRepository?.Read(request);
        return data ?? new List<MeasurementDataDTO>();
    }
}