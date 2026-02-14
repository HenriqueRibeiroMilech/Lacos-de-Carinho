using System.Net.Http.Json;
using System.Text.Json;
using Ldc.Domain.Services.Payment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ldc.Infrastructure.Services.Payment;

public class MercadoPagoService : IMercadoPagoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MercadoPagoService> _logger;
    private readonly string _accessToken;
    private readonly string _successUrl;
    private readonly string _failureUrl;
    private readonly string _pendingUrl;
    private readonly string _webhookUrl;

    public MercadoPagoService(IConfiguration configuration, HttpClient httpClient, ILogger<MercadoPagoService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _accessToken = configuration.GetValue<string>("Settings:MercadoPago:AccessToken") ?? "";
        _successUrl = configuration.GetValue<string>("Settings:MercadoPago:SuccessUrl") ?? "http://localhost:4200/pagamento-sucesso";
        _failureUrl = configuration.GetValue<string>("Settings:MercadoPago:FailureUrl") ?? "http://localhost:4200/pagamento-falha";
        _pendingUrl = configuration.GetValue<string>("Settings:MercadoPago:PendingUrl") ?? "http://localhost:4200/pagamento-pendente";
        _webhookUrl = configuration.GetValue<string>("Settings:MercadoPago:WebhookUrl") ?? "";
        
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<(string PreferenceId, string CheckoutUrl)> CreatePreference(
        string title,
        string description,
        decimal amount,
        string externalReference,
        string payerEmail)
    {
        // Só usa auto_return se as URLs forem HTTPS (produção)
        var useAutoReturn = _successUrl.StartsWith("https://");
        
        var preference = new Dictionary<string, object>
        {
            ["items"] = new[]
            {
                new Dictionary<string, object>
                {
                    ["title"] = title,
                    ["description"] = description,
                    ["quantity"] = 1,
                    ["currency_id"] = "BRL",
                    ["unit_price"] = (double)amount
                }
            },
            ["payer"] = new Dictionary<string, object>
            {
                ["email"] = payerEmail
            },
            ["back_urls"] = new Dictionary<string, string>
            {
                ["success"] = _successUrl,
                ["failure"] = _failureUrl,
                ["pending"] = _pendingUrl
            },
            ["external_reference"] = externalReference,
            ["payment_methods"] = new Dictionary<string, object>
            {
                ["excluded_payment_methods"] = Array.Empty<object>(),
                ["excluded_payment_types"] = Array.Empty<object>(),
                ["installments"] = 12
            }
        };
        
        if (useAutoReturn)
        {
            preference["auto_return"] = "approved";
        }
        
        if (!string.IsNullOrEmpty(_webhookUrl))
        {
            preference["notification_url"] = _webhookUrl;
        }

        _logger.LogInformation("Creating Mercado Pago preference for: {Email}, Amount: {Amount}", payerEmail, amount);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/checkout/preferences")
        {
            Content = JsonContent.Create(preference)
        };
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Mercado Pago API error: {StatusCode} - {Response}", response.StatusCode, responseContent);
            throw new Exception($"Erro ao criar preferência de pagamento: {responseContent}");
        }

        _logger.LogInformation("Mercado Pago preference created successfully");

        var json = JsonDocument.Parse(responseContent);
        
        var preferenceId = json.RootElement.GetProperty("id").GetString() ?? "";
        var checkoutUrl = json.RootElement.GetProperty("init_point").GetString() ?? "";

        return (preferenceId, checkoutUrl);
    }

    public async Task<MercadoPagoDirectPaymentResult> CreatePayment(
        string token,
        decimal amount,
        string description,
        string payerEmail,
        string externalReference,
        string paymentMethodId,
        string issuerId,
        int installments)
    {
        var paymentData = new Dictionary<string, object>
        {
            ["transaction_amount"] = (double)amount,
            ["description"] = description,
            ["payment_method_id"] = paymentMethodId,
            ["external_reference"] = externalReference,
            ["payer"] = new Dictionary<string, object>
            {
                ["email"] = payerEmail
            }
        };

        // Para Pix, não enviamos token nem issuer_id/installments
        if (paymentMethodId == "pix")
        {
            // Pix não precisa de token
        }
        else
        {
            // Pagamento com cartão: inclui token, issuer e parcelas
            paymentData["token"] = token;
            paymentData["issuer_id"] = issuerId;
            paymentData["installments"] = installments;
        }

        // Adiciona notification_url se configurado
        if (!string.IsNullOrEmpty(_webhookUrl))
        {
            paymentData["notification_url"] = _webhookUrl;
        }

        _logger.LogInformation(
            "Creating direct payment for: {Email}, Amount: {Amount}, Method: {Method}",
            payerEmail, amount, paymentMethodId);

        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.mercadopago.com/v1/payments")
        {
            Content = JsonContent.Create(paymentData)
        };
        request.Headers.Add("X-Idempotency-Key", Guid.NewGuid().ToString());

        var response = await _httpClient.SendAsync(request);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Mercado Pago payment error: {StatusCode} - {Response}",
                response.StatusCode, responseContent);
            throw new Exception($"Erro ao processar pagamento: {responseContent}");
        }

        var json = JsonDocument.Parse(responseContent);
        var root = json.RootElement;

        var paymentId = root.GetProperty("id").GetInt64();
        var status = root.GetProperty("status").GetString() ?? "unknown";
        var statusDetail = root.TryGetProperty("status_detail", out var sd)
            ? sd.GetString() ?? "" : "";

        // Para Pix, extrair dados do QR Code
        string? pixQrCode = null;
        string? pixQrCodeBase64 = null;
        string? ticketUrl = null;

        if (root.TryGetProperty("point_of_interaction", out var poi))
        {
            if (poi.TryGetProperty("transaction_data", out var txData))
            {
                if (txData.TryGetProperty("qr_code", out var qr))
                    pixQrCode = qr.GetString();
                if (txData.TryGetProperty("qr_code_base64", out var qrB64))
                    pixQrCodeBase64 = qrB64.GetString();
                if (txData.TryGetProperty("ticket_url", out var ticket))
                    ticketUrl = ticket.GetString();
            }
        }

        _logger.LogInformation(
            "Payment created: Id={PaymentId}, Status={Status}, StatusDetail={StatusDetail}",
            paymentId, status, statusDetail);

        return new MercadoPagoDirectPaymentResult(
            paymentId, status, statusDetail,
            pixQrCode, pixQrCodeBase64, ticketUrl);
    }

    public async Task<string> GetPaymentStatus(string paymentId)
    {
        var details = await GetPaymentDetails(paymentId);
        return details.Status;
    }

    public async Task<MercadoPagoPaymentDetails> GetPaymentDetails(string paymentId)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                $"https://api.mercadopago.com/v1/payments/{paymentId}");

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Could not get payment details for: {PaymentId}, StatusCode: {StatusCode}", 
                    paymentId, response.StatusCode);
                return new MercadoPagoPaymentDetails("unknown", "", null, null);
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);
            
            var status = json.RootElement.GetProperty("status").GetString() ?? "unknown";
            var externalReference = json.RootElement.TryGetProperty("external_reference", out var extRef) 
                ? extRef.GetString() ?? "" 
                : "";
            
            string? payerEmailResult = null;
            if (json.RootElement.TryGetProperty("payer", out var payer) && 
                payer.TryGetProperty("email", out var email))
            {
                payerEmailResult = email.GetString();
            }
            
            decimal? transactionAmount = null;
            if (json.RootElement.TryGetProperty("transaction_amount", out var amt))
            {
                transactionAmount = amt.GetDecimal();
            }

            _logger.LogInformation(
                "Payment {PaymentId} details: Status={Status}, ExternalRef={ExternalRef}", 
                paymentId, status, externalReference);

            return new MercadoPagoPaymentDetails(status, externalReference, payerEmailResult, transactionAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment details for: {PaymentId}", paymentId);
            return new MercadoPagoPaymentDetails("unknown", "", null, null);
        }
    }
}
