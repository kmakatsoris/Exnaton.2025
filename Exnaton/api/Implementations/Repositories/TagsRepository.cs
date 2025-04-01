using Exnaton.Exceptions;
using Exnaton.Interfaces.Repositories;
using Exnaton.Models;
using Exnaton.Models.Entities;

namespace Exnaton.Implementations.Repositories;

public class TagsRepository: Repository<TagsEntity>, ITagsRepository
{
    private readonly Serilog.ILogger _logger;
        
    public TagsRepository(AppDbContext context, Serilog.ILogger logger): base(context, logger)
    {
        _logger = logger;
    }

    public async Task<string?> ReadQualityFromTagMUId(Guid tagmuId)
    {
        if (tagmuId == Guid.Empty)
            return null;
        var result = await ReadFromPredicateAsync(t => t != null && t.Muid == tagmuId);
        return result?.Quality ?? "";
    }
}