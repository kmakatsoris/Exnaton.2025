using Exnaton.Models.Requests;

namespace Exnaton.Interfaces;

public interface IAnalysisService
{
    Task AnalysisMeasurements(ReadMeasurementsRequest request);
}