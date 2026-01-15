using Ldc.Domain.Entities;
using Ldc.Domain.Repositories.Payment;
using Microsoft.EntityFrameworkCore;

namespace Ldc.Infrastructure.DataAccess.Repositories;

internal class PaymentRepository : IPaymentRepository
{
    private readonly ApiDbContext _dbContext;

    public PaymentRepository(ApiDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Add(Payment payment)
    {
        await _dbContext.Payments.AddAsync(payment);
    }

    public async Task<Payment?> GetByPreferenceId(string preferenceId)
    {
        return await _dbContext.Payments
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PreferenceId == preferenceId);
    }

    public async Task<Payment?> GetByMercadoPagoPaymentId(string paymentId)
    {
        return await _dbContext.Payments
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.MercadoPagoPaymentId == paymentId);
    }

    public void Update(Payment payment)
    {
        _dbContext.Payments.Update(payment);
    }
}
