using Ldc.Communication.Enums;

namespace Ldc.Communication.Requests;

public class RequestUpdateGiftItemJson
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public GiftCategory? Category { get; set; }
    public GiftItemStatus? Status { get; set; }
}
