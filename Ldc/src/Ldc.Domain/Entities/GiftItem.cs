using Ldc.Domain.Enums;

namespace Ldc.Domain.Entities;

public class GiftItem
{
    public long Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public GiftCategory Category { get; set; } = GiftCategory.Outros;

    public GiftItemStatus Status { get; set; } = GiftItemStatus.Available;
    
    public long WeddingListId { get; set; }
    public WeddingList WeddingList { get; set; } = null!;
    
    public long? ReservedById { get; set; }
    public User? ReservedBy { get; set; }
}