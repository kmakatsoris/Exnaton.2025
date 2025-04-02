using Exnaton.Exceptions;
using Exnaton.Interfaces;
using Exnaton.Models;
using Exnaton.Models.Requests;
using Microsoft.Extensions.Options;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Implementations;

public class AnalysisService: IAnalysisService
{
    private readonly IDataService _dataService;
    private readonly Serilog.ILogger _logger;
    
    public AnalysisService(IDataService dataService, Serilog.ILogger logger)
    {
        _dataService =  dataService;
        _logger = logger;
    }
    
    public async Task AnalysisMeasurements(ReadMeasurementsRequest request)
    {
        if (request?.Validate() != true)
            throw BusinessExceptions.InvalidMeasurementException(_logger, reason: "Invalid request to Measurement Analysis.");
        List<MeasurementDataDTO> data = await _dataService?.ReadDataAsync(request);
        PDFUtils.ExportPDF(data, logger: _logger);
    }
}