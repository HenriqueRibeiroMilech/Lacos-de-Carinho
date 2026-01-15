using Ldc.Communication.Responses;

namespace Ldc.Application.UseCases.TemplateItems.GetAll;

public interface IGetAllTemplateItemsUseCase
{
    Task<ResponseTemplateItemsJson> Execute();
}
