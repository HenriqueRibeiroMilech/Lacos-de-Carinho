using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseWeddingListShortJson
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShareableLink { get; set; } = string.Empty;
    public DateTime? EventDate { get; set; }
    public string? Message { get; set; }
    public ListType ListType { get; set; }
    public int TotalItems { get; set; }
    public int ReservedItems { get; set; }
    public int TotalRsvps { get; set; }
    public int ConfirmedRsvps { get; set; }
}
