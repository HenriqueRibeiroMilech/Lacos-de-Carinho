using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseRsvpJson
{
    public long Id { get; set; }
    public long GuestId { get; set; }
    public string? GuestName { get; set; }
    public RsvpStatus Status { get; set; }
    public string? AdditionalGuests { get; set; }
}
