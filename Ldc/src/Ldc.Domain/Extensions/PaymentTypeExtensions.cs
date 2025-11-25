using Ldc.Domain.Enums;

namespace Ldc.Domain.Extensions;

public static class PaymentTypeExtensions
{
    public static string PaymentTypeToString(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.Cash => "Cash",
            PaymentType.CreditCard => "Credit Card",
            PaymentType.DebitCard => "Debit Card",
            PaymentType.ElectronicTransfer => "Electronic Transfer",
            _ => string.Empty
        };
    }
}