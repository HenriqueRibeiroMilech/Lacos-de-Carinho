using Ldc.Domain.Enums;

namespace Ldc.Domain.Repositories.WeddingList;

public interface IWeddingListReadOnlyRepository
{
    Task<Entities.WeddingList?> GetById(long id);
    Task<Entities.WeddingList?> GetByShareableLink(string shareableLink);
    Task<List<Entities.WeddingList>> GetAllByOwnerId(long ownerId);
    Task<List<Entities.WeddingList>> GetAllByOwnerIdWithDetails(long ownerId);
    Task<bool> ExistsById(long id);
    Task<bool> ExistsByOwnerAndType(long ownerId, ListType listType);
}
