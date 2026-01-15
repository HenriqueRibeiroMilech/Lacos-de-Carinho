using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseWeddingListJson
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? DeliveryInfo { get; set; }
    public string ShareableLink { get; set; } = string.Empty;
    public ListType ListType { get; set; }
    public bool IsOwner { get; set; }
    public List<ResponseGiftItemJson> Items { get; set; } = new();
    public List<ResponseRsvpJson> Rsvps { get; set; } = new();
}
