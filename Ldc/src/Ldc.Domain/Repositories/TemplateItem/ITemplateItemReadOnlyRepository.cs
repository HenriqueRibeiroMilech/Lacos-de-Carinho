using Ldc.Domain.Entities;

namespace Ldc.Domain.Repositories.TemplateItem;

public interface ITemplateItemReadOnlyRepository
{
    Task<List<TemplateGiftItem>> GetAllWithCategories();
}
