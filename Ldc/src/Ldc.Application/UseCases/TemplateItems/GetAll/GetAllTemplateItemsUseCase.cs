using Ldc.Communication.Responses;
using Ldc.Domain.Repositories.TemplateItem;

namespace Ldc.Application.UseCases.TemplateItems.GetAll;

public class GetAllTemplateItemsUseCase : IGetAllTemplateItemsUseCase
{
    private readonly ITemplateItemReadOnlyRepository _templateItemRepository;

    public GetAllTemplateItemsUseCase(ITemplateItemReadOnlyRepository templateItemRepository)
    {
        _templateItemRepository = templateItemRepository;
    }

    public async Task<ResponseTemplateItemsJson> Execute()
    {
        var items = await _templateItemRepository.GetAllWithCategories();

        var groups = items
            .GroupBy(i => i.Category.Id) // Group by ID to ensure items with same category are grouped together
            .OrderBy(g => g.First().Category.Name)
            .Select(g => new ResponseTemplateGroupJson
            {
                Category = new ResponseCategoryJson
                {
                    Id = g.Key,
                    Name = g.First().Category.Name
                },
                Items = g.Select(i => new ResponseTemplateGiftItemJson
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description
                }).ToList()
            })
            .ToList();

        return new ResponseTemplateItemsJson
        {
            Groups = groups
        };
    }
}
