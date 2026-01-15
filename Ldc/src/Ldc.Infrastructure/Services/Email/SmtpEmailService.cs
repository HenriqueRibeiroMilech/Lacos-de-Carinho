using System.Net;
using System.Net.Mail;
using Ldc.Domain.Services.Email;
using Microsoft.Extensions.Configuration;

namespace Ldc.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly bool _enableSsl;

    public SmtpEmailService(IConfiguration configuration)
    {
        var emailSettings = configuration.GetSection("Settings:Email");
        _smtpHost = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        _smtpUser = emailSettings["SmtpUser"] ?? "";
        _smtpPassword = emailSettings["SmtpPassword"] ?? "";
        _fromEmail = emailSettings["FromEmail"] ?? _smtpUser;
        _fromName = emailSettings["FromName"] ?? "Laços de Carinho";
        _enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
    {
        var subject = "Recuperação de Senha - Laços de Carinho";
        var body = GetPasswordResetEmailTemplate(userName, resetLink);

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        using var client = new SmtpClient(_smtpHost, _smtpPort)
        {
            Credentials = new NetworkCredential(_smtpUser, _smtpPassword),
            EnableSsl = _enableSsl
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mailMessage.To.Add(toEmail);

        await client.SendMailAsync(mailMessage);
    }

    private string GetPasswordResetEmailTemplate(string userName, string resetLink)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>
<body style='margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, ""Helvetica Neue"", Arial, sans-serif; background-color: #f8f4f5;'>
    <table role='presentation' style='width: 100%; border-collapse: collapse;'>
        <tr>
            <td align='center' style='padding: 40px 0;'>
                <table role='presentation' style='width: 100%; max-width: 600px; border-collapse: collapse; background-color: #ffffff; border-radius: 16px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);'>
                    <!-- Header -->
                    <tr>
                        <td style='padding: 40px 40px 20px; text-align: center; background: linear-gradient(135deg, #D97F97 0%, #c06f85 100%); border-radius: 16px 16px 0 0;'>
                            <h1 style='margin: 0; color: #ffffff; font-size: 28px; font-weight: 600;'>💝 Laços de Carinho</h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style='padding: 40px;'>
                            <h2 style='margin: 0 0 20px; color: #333333; font-size: 24px; font-weight: 600;'>Recuperação de Senha</h2>
                            <p style='margin: 0 0 20px; color: #666666; font-size: 16px; line-height: 1.6;'>
                                Olá, <strong>{userName}</strong>!
                            </p>
                            <p style='margin: 0 0 20px; color: #666666; font-size: 16px; line-height: 1.6;'>
                                Recebemos uma solicitação para redefinir a senha da sua conta. Clique no botão abaixo para criar uma nova senha:
                            </p>
                            
                            <!-- Button -->
                            <table role='presentation' style='width: 100%; border-collapse: collapse;'>
                                <tr>
                                    <td align='center' style='padding: 20px 0;'>
                                        <a href='{resetLink}' style='display: inline-block; padding: 16px 40px; background: linear-gradient(135deg, #D97F97 0%, #c06f85 100%); color: #ffffff; text-decoration: none; font-size: 16px; font-weight: 600; border-radius: 12px; box-shadow: 0 4px 12px rgba(217, 127, 151, 0.4);'>
                                            Redefinir Minha Senha
                                        </a>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style='margin: 20px 0 0; color: #999999; font-size: 14px; line-height: 1.6;'>
                                Este link expira em <strong>1 hora</strong>. Se você não solicitou a redefinição de senha, ignore este email.
                            </p>
                            
                            <hr style='margin: 30px 0; border: none; border-top: 1px solid #eeeeee;'>
                            
                            <p style='margin: 0; color: #999999; font-size: 12px; line-height: 1.6;'>
                                Se o botão não funcionar, copie e cole este link no seu navegador:<br>
                                <a href='{resetLink}' style='color: #D97F97; word-break: break-all;'>{resetLink}</a>
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style='padding: 20px 40px; text-align: center; background-color: #faf8f9; border-radius: 0 0 16px 16px;'>
                            <p style='margin: 0; color: #999999; font-size: 12px;'>
                                © 2025 Laços de Carinho. Todos os direitos reservados.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>";
    }
}
