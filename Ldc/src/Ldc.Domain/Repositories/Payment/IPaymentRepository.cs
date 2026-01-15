using Ldc.Domain.Entities;

namespace Ldc.Domain.Repositories.Payment;

public interface IPaymentRepository
{
    Task Add(Entities.Payment payment);
    Task<Entities.Payment?> GetByPreferenceId(string preferenceId);
    Task<Entities.Payment?> GetByMercadoPagoPaymentId(string paymentId);
    void Update(Entities.Payment payment);
}
