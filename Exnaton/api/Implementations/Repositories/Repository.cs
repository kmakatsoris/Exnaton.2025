using System.Linq.Expressions;
using System.Text.Json;
using Exnaton.Exceptions;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models.Entities;
using Exnaton.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace Exnaton.Implementations.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly Serilog.ILogger _logger;

    public Repository(AppDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public async Task<IEnumerable<T>> ReadAllAsync(int? limit)
    {
        try
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (limit!=null && limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }
    }

    public async Task<T?> ReadByIdAsync(Guid id)
    {
        try
        {
            if (id == Guid.Empty)
                throw BusinessExceptions.InvalidMeasurementException(_logger, $"Invalid ID: {id}");

            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }
    }

    public async Task<T?> ReadFromPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            if (predicate == null)
                throw BusinessExceptions.InvalidMeasurementException(_logger, reason: "Predicate is invalid.", logMsg: $"{JsonSerializer.Serialize(predicate?.ToString())}");
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }
    }
    
    public async Task<List<T>> ReadAllFromPredicateAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            if (predicate == null)
                return null;
            return await _dbSet.Where(predicate).ToListAsync();
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }    
    }
    
    public virtual async Task CreateAsync(IEnumerable<T> entities)
    {
        try
        {
            if (entities == null || !entities.Any())
                throw new ArgumentNullException(nameof(entities), "Entity list cannot be null or empty.");

            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }
    }

    public async Task CreateAsync(T entity)
    {
        try
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }    
    }

    public async Task UpdateAsync(T entity, Expression<Func<T, bool>>? predicate = null, T? oldEntity = null)
    {
        try
        {
            if (entity == null)
                throw BusinessExceptions.InvalidMeasurementException(_logger, $"Invalid entity: {nameof(entity)}");

            var existingEntity = oldEntity ?? (predicate != null ? await ReadFromPredicateAsync(predicate) : null);

            if (existingEntity != null)
            {
                var entry = _context.Entry(existingEntity);
                var entityEntry = _context.Entry(entity);

                foreach (var property in entityEntry.Properties)
                {
                    if (!property.Metadata.IsPrimaryKey())
                    {
                        var currentValue = property.CurrentValue;
                        var propertyName = property.Metadata.Name;

                        // Handle foreign key properties
                        if (property.Metadata.IsForeignKey()) 
                        {
                            var foreignKey = property.Metadata.GetContainingForeignKeys().FirstOrDefault();
                            if (foreignKey != null)
                            {
                                var relatedEntityType = foreignKey.PrincipalEntityType.ClrType;
                                var newFkValue = entityEntry.Property(propertyName).CurrentValue;

                                // Ensure the related entity exists before assigning
                                var relatedEntity = await _context.FindAsync(relatedEntityType, newFkValue);
                                if (relatedEntity == null)
                                    throw new Exception($"Related entity of type {relatedEntityType.Name} not found. Make sure it exists in the database.");

                                entry.Property(propertyName).CurrentValue = newFkValue;
                            }
                        }
                        else
                        {
                            entry.Property(propertyName).CurrentValue = currentValue;
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                throw BusinessExceptions.NotEntityFoundException(_logger, logMsg: nameof(entity));
            }
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }      
    }
    
    public async Task DeletePredicateAsync(Expression<Func<T, bool>> predicate)
    {
        try
        {
            var entity = await ReadFromPredicateAsync(predicate);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            throw BusinessExceptions.ServiceNotAvailableException(_logger, logMsg: ex.Message);
        }       
    }

    #region MyRegion
    private async Task<List<MeasurementDataEntity>> ReadAllLimitedMeasurementsWithLimittedTagsAsync(int measurementLimit, int tagsLimit)
    {
        if (measurementLimit <= 0 || tagsLimit <= 0) return new List<MeasurementDataEntity>();
        var sqlQuery = @"
                SELECT m.*, t.*
                FROM measurements m
                LEFT JOIN (
                    SELECT * FROM tags WHERE muid IN (
                        SELECT DISTINCT tagsmuid FROM measurements 
                        ORDER BY rectimestamp DESC 
                        LIMIT @MeasurementLimit
                    ) 
                    LIMIT @TagsLimit
                ) AS t ON m.tagsmuid = t.muid
                ORDER BY m.rectimestamp DESC
                LIMIT @MeasurementLimit;
            ";
        
        var measurements = await _context.MeasurementData
            .FromSqlRaw(sqlQuery, 
                new MySqlParameter("@MeasurementLimit", measurementLimit),
                new MySqlParameter("@TagsLimit", tagsLimit))
            .AsNoTracking()
            .ToListAsync();

        return measurements;
    }
    
    private async Task<List<MeasurementDataEntity>> ReadByIdLimitedMeasurementsWithLimittedTagsAsync(int recordId, int measurementLimit, int tagsLimit)
    {
        if (measurementLimit <= 0 || tagsLimit <= 0) return new List<MeasurementDataEntity>();
        var sqlQuery = @"
                SELECT m.*, t.*
                FROM measurements m
                LEFT JOIN (
                    SELECT * FROM tags WHERE muid IN (
                        SELECT DISTINCT tagsmuid FROM measurements 
                        WHERE Id = @RecordId
                        ORDER BY rectimestamp DESC 
                        LIMIT @MeasurementLimit
                    ) 
                    LIMIT @TagsLimit
                ) AS t ON m.tagsmuid = t.muid
                WHERE Id = @RecordId
                ORDER BY m.rectimestamp DESC
                LIMIT @MeasurementLimit;
            ";
        
        var measurements = await _context.MeasurementData
            .FromSqlRaw(sqlQuery, 
                new MySqlParameter("@RecordId", recordId),
                new MySqlParameter("@MeasurementLimit", measurementLimit),
                new MySqlParameter("@TagsLimit", tagsLimit))
            .AsNoTracking()
            .ToListAsync();

        return measurements;
    }
    
    private async Task<List<MeasurementDataEntity>> ReadByFullClauseLimitedMeasurementsWithLimittedTagsAsync(int recordId, int measurementLimit, int tagsLimit)
    {
        if (measurementLimit <= 0 || tagsLimit <= 0) return new List<MeasurementDataEntity>();
        var sqlQuery = @"
                SELECT m.*, t.*
                FROM measurements m
                LEFT JOIN (
                    SELECT * FROM tags 
                    WHERE muid = @MUID 
                    LIMIT @TagsLimit
                ) AS t ON m.tagsmuid = t.muid
                WHERE m.measurement = @MEASUREMENT
                ORDER BY m.rectimestamp DESC
                LIMIT @MeasurementLimit;
            ";
        
        var measurements = await _context.MeasurementData
            .FromSqlRaw(sqlQuery, 
                new MySqlParameter("@RecordId", recordId),
                new MySqlParameter("@MeasurementLimit", measurementLimit),
                new MySqlParameter("@TagsLimit", tagsLimit))
            .AsNoTracking()
            .ToListAsync();

        return measurements;
    }

    #endregion
}