using Ldc.Communication.Enums;

namespace Ldc.Communication.Requests;

public class RequestCreateWeddingListJson
{
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? DeliveryInfo { get; set; }
    public ListType ListType { get; set; } = ListType.Wedding;
}
