using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseGuestEventRsvpJson
{
    public ResponseRsvpJson Rsvp { get; set; } = null!;
    public ResponseWeddingListShortJson WeddingList { get; set; } = null!;
}
