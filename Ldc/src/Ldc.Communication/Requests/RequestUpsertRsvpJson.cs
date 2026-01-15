using Ldc.Communication.Enums;

namespace Ldc.Communication.Requests;

public class RequestUpsertRsvpJson
{
    public RsvpStatus Status { get; set; }
    public string? AdditionalGuests { get; set; }
}
