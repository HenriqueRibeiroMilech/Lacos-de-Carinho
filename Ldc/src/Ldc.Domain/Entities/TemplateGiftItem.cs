namespace Ldc.Domain.Entities;

public class TemplateGiftItem
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public long CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}
