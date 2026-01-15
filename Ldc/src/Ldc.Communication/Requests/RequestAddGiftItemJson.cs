using Ldc.Communication.Enums;

namespace Ldc.Communication.Requests;

public class RequestAddGiftItemJson
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GiftCategory Category { get; set; } = GiftCategory.Outros;
}
