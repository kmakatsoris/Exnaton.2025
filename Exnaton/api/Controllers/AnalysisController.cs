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
    public async Task<IActionResult> AnalysisMeasurements([FromBody] ReadMeasurementsRequest request)
    {
        await _analysisService?.AnalysisMeasurements(request);
        var pdfFullname = Path.Combine(Directory.GetCurrentDirectory(), $"exploitation_results_{request?.Muid}_{request?.Limit}.pdf");
        return Ok($"PDF evaluation is available at: {pdfFullname}");
    }
}