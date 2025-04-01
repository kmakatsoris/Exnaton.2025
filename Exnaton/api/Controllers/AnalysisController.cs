using Exnaton.Interfaces;
using Exnaton.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.api.Controllers;

[Route("[controller]")]
[ApiController]
public class AnalysisController: ControllerBase
{
    private readonly Serilog.ILogger _logger;
    private readonly IAnalysisService _analysisService;
    
    public AnalysisController(Serilog.ILogger logger, IAnalysisService analysisService)
    {
        _logger = logger;    
        _analysisService = analysisService;
    }

    [HttpPost("measurements")]
    public async Task AnalysisMeasurements([FromBody] ReadMeasurementsRequest request) =>
        await _analysisService?.AnalysisMeasurements(request);
}