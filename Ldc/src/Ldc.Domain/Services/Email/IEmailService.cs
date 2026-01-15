namespace Ldc.Domain.Services.Email;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
}
