namespace Ldc.Domain.Entities;

public class Category
{
    public long Id { get; set; }
    
    public string Name { get; set; } = string.Empty;
    
    public ICollection<TemplateGiftItem> TemplateItems { get; set; } = new List<TemplateGiftItem>();
}
