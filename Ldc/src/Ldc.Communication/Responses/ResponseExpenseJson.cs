using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseExpenseJson
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public PaymentType PaymentType { get; set; }
    public IList<Tag> Tags { get; set; } = [];
}