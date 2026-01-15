using Ldc.Domain.Enums;

namespace Ldc.Domain.Entities;

public class WeddingList
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string? Message { get; set; }
    
    public string? DeliveryInfo { get; set; }
    
    public DateOnly EventDate { get; set; } 
    
    public ListType ListType { get; set; } = ListType.Wedding;
    
    public string ShareableLink { get; set; } = string.Empty;
    
    public long OwnerId { get; set; }
    public User Owner { get; set; } = null!; 
    
    public ICollection<GiftItem> Items { get; set; } = new List<GiftItem>();
    
    public ICollection<Rsvp> Rsvps { get; set; } = new List<Rsvp>();
}