using System.Text.Json.Serialization;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Models.Requests;

public class BaseMeasurementsRequest
{
    [JsonPropertyName("muid")]
    public Guid Muid { get; set; }

    public bool Validate() =>
        Muid != Guid.Empty;
}

public class ReadMeasurementsRequest:  BaseMeasurementsRequest
{
    [JsonPropertyName("measurement")]
    public string Measurement { get; set; } 
    
    [JsonPropertyName("limit")]
    public int Limit { get; set; } 
    
    [JsonPropertyName("start")]
    public DateTime Start { get; set; } 
    
    [JsonPropertyName("stop")]
    public DateTime Stop { get; set; } 

    public bool Validate() => 
        base.Validate() &&
        !string.IsNullOrEmpty(Measurement) && 
        Limit > 0 || 
        Start <= Stop;

    public string LogString() =>
        $"muid == Guid.Empty ({Muid == Guid.Empty}) || string.IsNullOrEmpty(measurement) ({string.IsNullOrEmpty(Measurement)}) || limit <= 0 ({Limit <= 0}) || start <= stop ({Start <= Stop})";
    
}
