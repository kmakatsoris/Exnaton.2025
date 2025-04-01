using System.Text.Json;
using System.Text.Json.Serialization;
using Exnaton.Exceptions;
using Exnaton.Models.Entities;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models;
using Exnaton.Models.Requests;
using Microsoft.EntityFrameworkCore;
using Models.DTOs.MeasurementDataDTO;

namespace Exnaton.Implementations.Repositories;

public class MeasurementsRepository: Repository<MeasurementDataEntity>, IMeasurementsRepository
{
    private readonly Serilog.ILogger _logger;
        
    public MeasurementsRepository(AppDbContext context, Serilog.ILogger logger): base(context, logger)
    {
        _logger = logger;
    }

    public List<MeasurementDataDTO>? Read(ReadMeasurementsRequest request)
    {
        if (request?.Validate() != true)
            throw BusinessExceptions.InvalidMeasurementException(_logger, reason: "Invalid arguments when trying to Read the measurement data.", 
                logMsg: request?.LogString());
        return _dbSet?.AsNoTracking()
                     ?.Where(m => m != null && 
                                                     m.TagsMUId == request.Muid && 
                                                     m.Measurement == request.Measurement && 
                                                     m.Timestamp >= request.Start && m.Timestamp <= request.Stop)
            ?.OrderByDescending(m => m.Timestamp) 
            ?.Take(request.Limit)
            ?.Select(m => (MeasurementDataDTO)m)
            ?.ToList();
    }
    
    public async Task<List<MeasurementDataEntity>> AddAsync(List<MeasurementDataDTO> measurementData)
    {
        if (measurementData == null || measurementData?.Any(m => m?.Validate() != true) == true)
            throw BusinessExceptions.InvalidMeasurementException(_logger, reason: "Invalid arguments when trying to Read the measurement data.", 
                logMsg: $"MeasurementDataDTO: {JsonSerializer.Serialize(measurementData)}");
        List<MeasurementDataEntity> entities = measurementData?
            .Select(m => (MeasurementDataEntity)m)
            .ToList();
        if (entities == null || entities?.Any(e => e == null) == true)
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: $"Could not convert MeasurementDataDTO to MeasurementDataEntity. MeasurementDataDTO: {JsonSerializer.Serialize(measurementData)}");
        return await CreateAsync(entities);
    }
    
    // public T DeepCopy<T>(T obj)
    // {
    //     // Serialize the object to JSON
    //     string json = JsonSerializer.Serialize(obj);
    //
    //     // Deserialize back to a new object
    //     return JsonSerializer.Deserialize<T>(json);
    // }
    
    
    public override async Task<List<MeasurementDataEntity>> CreateAsync(IEnumerable<MeasurementDataEntity> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                throw BusinessExceptions.InvalidMeasurementException(_logger, "Entity list cannot be null or empty.");
            
            // Because we removed the foreign key for performance reasons
            var tagIds = await _context.Tags
                .ToDictionaryAsync(t => t.Id, t => t.Muid);
            
            List<MeasurementDataEntity> entitiesToCreate = new List<MeasurementDataEntity>();
            foreach (var measurementEntity in entities)
            {
                if (!tagIds.ContainsValue(measurementEntity.TagsMUId))
                {
                    continue;
                }
                entitiesToCreate.Add(measurementEntity);
            }

            // MeasurementDataEntity[] t = DeepCopy(entitiesToCreate.ToArray());
            if (entitiesToCreate?.Count > 0)
            {
                await _dbSet.AddRangeAsync(entitiesToCreate);
                await _context.SaveChangesAsync();
            }
            
            return entitiesToCreate;
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }
    }

}