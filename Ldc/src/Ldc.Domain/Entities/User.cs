using Ldc.Domain.Enums;

namespace Ldc.Domain.Entities;

public class User
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid UserIdentifier {get; set; }
    public string Role { get; set; } = Roles.USER;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<WeddingList> WeddingLists { get; set; } = new List<WeddingList>();
}