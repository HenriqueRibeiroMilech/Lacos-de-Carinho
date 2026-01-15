namespace Ldc.Communication.Responses;

public class ResponseGuestDetailsJson
{
    public long UserId { get; set; }
    public List<ResponseGuestEventRsvpJson> Events { get; set; } = new();
}
