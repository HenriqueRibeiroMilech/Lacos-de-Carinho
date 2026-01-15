namespace Ldc.Communication.Responses;

public class ResponseTrackingJson
{
    public long ListId { get; set; }
    public List<ResponseGiftTrackingJson> Gifts { get; set; } = new();
    public List<ResponseRsvpJson> Rsvps { get; set; } = new();
}
