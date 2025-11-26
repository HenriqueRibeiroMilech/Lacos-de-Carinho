using Ldc.Domain.Enums;

namespace Ldc.Domain.Entities;

public class Rsvp
{
    public long Id { get; set; }
    
    public long WeddingListId { get; set; }
    public WeddingList WeddingList { get; set; } = null!;
    
    public long GuestId { get; set; }
    public User Guest { get; set; } = null!;
    
    public RsvpStatus Status { get; set; } = RsvpStatus.Pending;
    
    public string? AdditionalGuests { get; set; }
}