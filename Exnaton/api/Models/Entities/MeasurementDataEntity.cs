using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Models.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id { get; set; }
}

[Table("measurements")]
public class MeasurementDataEntity: BaseEntity
{
    [Column("measurement")]
    public string Measurement { get; set; }
    
    [Column("rectimestamp")]
    public DateTime Timestamp { get; set; }
    
    [Column("tagsmuid")]
    public Guid TagsMUId { get; set; }
    
    // [ForeignKey("TagsMUId")]
    // public TagsEntity Tags { get; set; }
    
    [Column("indications", TypeName = "json")]
    public string IndicationsJson
    {
        get => JsonSerializer.Serialize(Indications);
        set => Indications = JsonSerializer.Deserialize<Dictionary<string, double>>(value) ?? new();
    }

    [NotMapped]  // Ignore in EF mapping
    public Dictionary<string, double> Indications { get; set; } = new();
    
    public static explicit operator MeasurementDataEntity(MeasurementDataDTO? entity)
    {
        if (entity == null)
            return null;
        var tags = (TagsEntity)entity.Tags;
        return tags == null ? null : new MeasurementDataEntity()
        {
            Measurement = entity.Measurement,
            Timestamp = entity.Timestamp,
            TagsMUId = tags.Muid,
            // Tags = tags,
            Indications = entity.IndicationsRTU,
        };
    } 
}

[Table("tags")]
public class TagsEntity: BaseEntity
{
    [Required]
    [Column("muid")]
    public Guid Muid { get; set; }
    
    [Column("quality")]
    public string Quality { get; set; }
    
    public static explicit operator TagsEntity(TagsDTO? entity)
    {
        if (entity == null)
            return null;
        return new TagsEntity()
        {
            Muid = entity.Muid,
            Quality = entity.Quality,
        };
    } 
}
