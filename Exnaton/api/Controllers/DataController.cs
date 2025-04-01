using Exnaton.Interfaces;
using Exnaton.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Controllers;

[Route("meterdata")]
[ApiController]
public class DataController: ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IDataService _dataService;
    
    public DataController(Serilog.ILogger logger, IDataService dataService)
    {
        _logger = logger;    
        _dataService = dataService;
    }

    [HttpGet("measurement")]
    public async Task<List<MeasurementDataDTO>> FetchMeasurements([FromQuery] Guid muid, [FromQuery] string measurement, [FromQuery] int limit, [FromQuery] DateTime start, [FromQuery] DateTime stop)
    {
        ReadMeasurementsRequest request = new ReadMeasurementsRequest
        {
            Muid = muid,
            Measurement = measurement,
            Limit = limit,
            Start = start,
            Stop = stop
        };
        var results = (await _dataService?.FetchAndStoreDataAsync(request, enReturnResponse: true)) ?? new List<MeasurementDataDTO>();
        return results;
    }

    [HttpPost("read")]
    public async Task<List<MeasurementDataDTO>> ReadMeasurements([FromBody] ReadMeasurementsRequest request)
    {
        var results = await _dataService?.ReadDataAsync(request) ?? new List<MeasurementDataDTO>();
        return results;
    }
}