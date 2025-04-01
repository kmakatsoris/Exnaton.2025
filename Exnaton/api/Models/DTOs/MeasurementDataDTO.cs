using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Exnaton.Models.Entities;
using Exnaton.Models.Requests;

namespace Models.DTOs.MeasurementDataDTO;


public class MeasurementDataWrapperDTO
{
    [JsonPropertyName("data")]
    public List<MeasurementDataDTO> Data { get; set; }
}

public class MeasurementDataDTO
{
    [JsonPropertyName("measurement")]
    public string Measurement { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; set; }

    [JsonPropertyName("tags")]
    public TagsDTO Tags { get; set; }

    [JsonExtensionData]
    public Dictionary<string, JsonElement> Indications { get; set; } = new Dictionary<string, JsonElement>();
    
    [JsonIgnore]
    public Dictionary<string, double> IndicationsRTU => ConvertIndications();

    private Dictionary<string, double> ConvertIndications()
    {
        var result = new Dictionary<string, double>();
        foreach (var kvp in Indications)
        {
            if (kvp.Value.ValueKind == JsonValueKind.Number && kvp.Value.TryGetDouble(out double value))
            {
                result[kvp.Key] = value;
            }
            else
            {
                Console.WriteLine($"Skipping key '{kvp.Key}' due to invalid value.");
            }
        }
        return result;
    }
    
    public bool Validate() => 
        !string.IsNullOrEmpty(Measurement) && 
        Timestamp != null &&
        Tags.Validate() &&
        Indications.Count > 0;
    
    public static explicit operator MeasurementDataDTO(MeasurementDataEntity? entity)
    {
        if (entity == null)
            return null;
        return new MeasurementDataDTO()
        {
            Measurement = entity.Measurement,
            Timestamp = entity.Timestamp,
            Tags = new TagsDTO()
            {
                Muid = entity.TagsMUId,
                Quality = ""
            },
            Indications = ConvertIndicationsToJsonElement(entity.Indications)
        };
    }
    
    private static Dictionary<string, JsonElement> ConvertIndicationsToJsonElement(Dictionary<string, double> indications)
    {
        var jsonElements = new Dictionary<string, JsonElement>();
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        foreach (var kvp in indications)
        {
            var json = JsonSerializer.Serialize(kvp.Value, options);
            jsonElements[kvp.Key] = JsonDocument.Parse(json).RootElement;
        }
        return jsonElements;
    }
}

public class TagsDTO
{
    [JsonPropertyName("muid")]
    public Guid Muid { get; set; }

    [JsonPropertyName("quality")]
    public string Quality { get; set; }
    
    public bool Validate() => 
        Muid != Guid.Empty &&
        !string.IsNullOrEmpty(Quality);
}

