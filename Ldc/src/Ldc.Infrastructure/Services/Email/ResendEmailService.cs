using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Ldc.Domain.Services.Email;
using Microsoft.Extensions.Configuration;

namespace Ldc.Infrastructure.Services.Email;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public ResendEmailService(IConfiguration configuration, HttpClient httpClient)
    {
        _httpClient = httpClient;
        var emailSettings = configuration.GetSection("Settings:Email");
        _apiKey = emailSettings["ResendApiKey"] ?? "";
        _fromEmail = emailSettings["FromEmail"] ?? "noreply@lacosdecarinho.com";
        _fromName = emailSettings["FromName"] ?? "Laços de Carinho";
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
    {
        var subject = "Recuperação de Senha - Laços de Carinho";
        var body = GetPasswordResetEmailTemplate(userName, resetLink);

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var requestBody = new
        {
            from = $"{_fromName} <{_fromEmail}>",
            to = new[] { toEmail },
            subject = subject,
            html = htmlBody
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Failed to send email via Resend: {response.StatusCode} - {errorContent}");
        }
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
                            <h1 style='margin: 0; color: #ffffff; font-size: 28px; font-weight: 600;'>Laços de Carinho</h1>
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
