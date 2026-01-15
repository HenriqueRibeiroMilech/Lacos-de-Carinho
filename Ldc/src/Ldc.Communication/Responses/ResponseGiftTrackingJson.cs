namespace Ldc.Communication.Responses;

public class ResponseGiftTrackingJson
{
    public long GiftItemId { get; set; }
    public string GiftName { get; set; } = string.Empty;
    public long? ReservedById { get; set; }
    public string? ReservedByName { get; set; }
}
