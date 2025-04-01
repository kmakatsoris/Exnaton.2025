namespace Exnaton.Interfaces.Repositories;

public interface ITagsRepository
{
    Task<string?> ReadQualityFromTagMUId(Guid tagmuId);
}