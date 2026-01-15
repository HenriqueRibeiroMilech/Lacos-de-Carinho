using Ldc.Communication.Enums;

namespace Ldc.Communication.Responses;

public class ResponseGiftItemJson
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GiftCategory Category { get; set; }
    public GiftItemStatus Status { get; set; }
    public long? ReservedById { get; set; }
    public string? ReservedByName { get; set; }
    public long? MyReservationId { get; set; } // Para convidados
}
