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
                ["success"] = $"{_successUrl}?external_reference={externalReference}",
                ["failure"] = $"{_failureUrl}?external_reference={externalReference}",
                ["pending"] = $"{_pendingUrl}?external_reference={externalReference}"
            },
            ["external_reference"] = externalReference
        };
        
        // Só adiciona auto_return se for HTTPS
        if (useAutoReturn)
        {
            preference["auto_return"] = "approved";
        }
        
        // Só adiciona notification_url se estiver configurado
        if (!string.IsNullOrEmpty(_webhookUrl))
        {
            preference["notification_url"] = _webhookUrl;
        }

        _logger.LogInformation("Creating Mercado Pago preference for: {Email}, Amount: {Amount}", payerEmail, amount);

        var response = await _httpClient.PostAsJsonAsync(
            "https://api.mercadopago.com/checkout/preferences",
            preference);

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
            
            string? payerEmail = null;
            if (json.RootElement.TryGetProperty("payer", out var payer) && 
                payer.TryGetProperty("email", out var email))
            {
                payerEmail = email.GetString();
            }
            
            decimal? transactionAmount = null;
            if (json.RootElement.TryGetProperty("transaction_amount", out var amount))
            {
                transactionAmount = amount.GetDecimal();
            }

            _logger.LogInformation(
                "Payment {PaymentId} details: Status={Status}, ExternalRef={ExternalRef}", 
                paymentId, status, externalReference);

            return new MercadoPagoPaymentDetails(status, externalReference, payerEmail, transactionAmount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting payment details for: {PaymentId}", paymentId);
            return new MercadoPagoPaymentDetails("unknown", "", null, null);
        }
    }
}
