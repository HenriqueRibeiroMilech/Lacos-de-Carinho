namespace Ldc.Communication.Responses;

public class ResponseTemplateGroupJson
{
    public ResponseCategoryJson Category { get; set; } = null!;
    public List<ResponseTemplateGiftItemJson> Items { get; set; } = new();
}
