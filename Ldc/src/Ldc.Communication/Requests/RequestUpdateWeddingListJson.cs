namespace Ldc.Communication.Requests;

public class RequestUpdateWeddingListJson
{
    public string? Title { get; set; }
    public string? Message { get; set; }
    public DateOnly? EventDate { get; set; }
    public string? DeliveryInfo { get; set; }
}
