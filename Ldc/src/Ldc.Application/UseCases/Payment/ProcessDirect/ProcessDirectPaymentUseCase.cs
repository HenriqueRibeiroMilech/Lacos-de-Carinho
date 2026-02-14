using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Domain.Services.Payment;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Ldc.Application.UseCases.Payment.ProcessDirect;

public class ProcessDirectPaymentUseCase : IProcessDirectPaymentUseCase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IUserUpdateOnlyRepository _userUpdateOnlyRepository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IAccessTokenGenerator _tokenGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly ILogger<ProcessDirectPaymentUseCase> _logger;
    private readonly decimal _price;

    public ProcessDirectPaymentUseCase(
        IMercadoPagoService mercadoPagoService,
        IPaymentRepository paymentRepository,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IUserUpdateOnlyRepository userUpdateOnlyRepository,
        IPasswordEncrypter passwordEncrypter,
        IAccessTokenGenerator tokenGenerator,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser,
        ILogger<ProcessDirectPaymentUseCase> logger,
        IConfiguration configuration)
    {
        _mercadoPagoService = mercadoPagoService;
        _paymentRepository = paymentRepository;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _userUpdateOnlyRepository = userUpdateOnlyRepository;
        _passwordEncrypter = passwordEncrypter;
        _tokenGenerator = tokenGenerator;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _logger = logger;
        _price = configuration.GetValue<decimal>("Settings:MercadoPago:Price", 49.90m);
    }

    public async Task<ResponseDirectPaymentJson> Execute(RequestProcessDirectPaymentJson request)
    {
        User user;
        string paymentType = request.PaymentType.ToLower();

        if (paymentType == "upgrade")
        {
            // Upgrade: usuário já está logado
            user = await _loggedUser.Get();

            if (user.Role == Roles.ADMIN)
            {
                throw new ErrorOnValidationException(["Você já possui uma conta de organizador."]);
            }
        }
        else
        {
            // Novo cadastro: criar ou reutilizar usuário
            var existingUser = await _userReadOnlyRepository.GetUserByEmail(request.PayerEmail);
            if (existingUser != null)
            {
                // Permite sobrescrever conta recente não-admin (tentativa anterior falhou)
                if (existingUser.Role == Roles.USER &&
                    existingUser.CreatedAt > DateTime.UtcNow.AddHours(-1))
                {
                    await _userWriteOnlyRepository.Delete(existingUser);
                    await _unitOfWork.Commit();
                }
            }

            await ValidateNewAccount(request);

            user = new User
            {
                Name = request.Name!,
                Email = request.PayerEmail,
                Password = _passwordEncrypter.Encrypt(request.Password!),
                UserIdentifier = Guid.NewGuid(),
                Role = Roles.USER
            };

            await _userWriteOnlyRepository.Add(user);
            await _unitOfWork.Commit();
        }

        // Referência interna para rastrear o pagamento
        var internalReference = Guid.NewGuid().ToString("N");

        _logger.LogInformation(
            "Processing direct payment for user {UserId}, type: {PaymentType}, method: {Method}",
            user.Id, paymentType, request.PaymentMethodId);

        // Processar pagamento direto no Mercado Pago
        var result = await _mercadoPagoService.CreatePayment(
            token: request.Token ?? "",
            amount: _price,
            description: "Laços de Carinho - Plano Organizador",
            payerEmail: paymentType == "upgrade" ? user.Email : request.PayerEmail,
            externalReference: internalReference,
            paymentMethodId: request.PaymentMethodId,
            issuerId: request.IssuerId,
            installments: request.Installments);

        // Salvar registro de pagamento
        var payment = new Domain.Entities.Payment
        {
            UserId = user.Id,
            PreferenceId = internalReference,
            MercadoPagoPaymentId = result.PaymentId.ToString(),
            Status = result.Status,
            Amount = _price,
            Description = "Plano Organizador - Laços de Carinho",
            PaymentType = paymentType,
            PaidAt = result.Status == "approved" ? DateTime.UtcNow : null
        };

        await _paymentRepository.Add(payment);
        await _unitOfWork.Commit();

        // Montar resposta
        var response = new ResponseDirectPaymentJson
        {
            Status = result.Status,
            StatusDetail = result.StatusDetail,
            PaymentId = result.PaymentId,
            PixQrCode = result.PixQrCode,
            PixQrCodeBase64 = result.PixQrCodeBase64,
            TicketUrl = result.TicketUrl
        };

        switch (result.Status)
        {
            case "approved":
                // Atualizar role para ADMIN imediatamente
                var userToUpdate = await _userUpdateOnlyRepository.GetById(user.Id);
                userToUpdate.Role = Roles.ADMIN;
                _userUpdateOnlyRepository.Update(userToUpdate);
                await _unitOfWork.Commit();

                // Gerar novo JWT com role ADMIN
                response.Token = _tokenGenerator.Generate(userToUpdate);
                response.Name = userToUpdate.Name;
                response.Message = "Pagamento aprovado! Bem-vindo ao Laços de Carinho.";

                _logger.LogInformation("Payment approved, user {UserId} upgraded to ADMIN", user.Id);
                break;

            case "pending":
            case "in_process":
                response.Message = request.PaymentMethodId == "pix"
                    ? "Pix gerado! Escaneie o QR Code ou copie o código para pagar."
                    : "Pagamento em processamento. Aguarde a confirmação.";
                break;

            case "rejected":
                response.Message = GetRejectionMessage(result.StatusDetail);
                break;

            default:
                response.Message = "Status do pagamento: " + result.Status;
                break;
        }

        return response;
    }

    private static string GetRejectionMessage(string statusDetail)
    {
        return statusDetail switch
        {
            "cc_rejected_insufficient_amount" => "Saldo insuficiente no cartão.",
            "cc_rejected_bad_filled_security_code" => "Código de segurança inválido.",
            "cc_rejected_bad_filled_date" => "Data de validade inválida.",
            "cc_rejected_bad_filled_other" => "Verifique os dados do cartão.",
            "cc_rejected_call_for_authorize" => "Ligue para sua operadora para autorizar o pagamento.",
            "cc_rejected_card_disabled" => "Seu cartão está desabilitado. Ligue para a operadora.",
            "cc_rejected_duplicated_payment" => "Pagamento duplicado. Aguarde a confirmação do anterior.",
            "cc_rejected_max_attempts" => "Limite de tentativas atingido. Tente outro cartão.",
            _ => "Pagamento não aprovado. Tente novamente com outro método de pagamento."
        };
    }

    private async Task ValidateNewAccount(RequestProcessDirectPaymentJson request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(ResourceErrorMessages.NAME_EMPTY);

        if (string.IsNullOrWhiteSpace(request.PayerEmail))
            errors.Add(ResourceErrorMessages.EMAIL_EMPTY);

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            errors.Add(ResourceErrorMessages.INVALID_PASSWORD);

        if (!string.IsNullOrWhiteSpace(request.PayerEmail))
        {
            var emailExists = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.PayerEmail);
            if (emailExists)
                errors.Add(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
        }

        if (errors.Any())
            throw new ErrorOnValidationException(errors);
    }
    public async Task<ResponseDirectPaymentJson> CheckPixStatus(long mpPaymentId)
    {
        // Buscar status diretamente no Mercado Pago
        var mpStatus = await _mercadoPagoService.GetPaymentStatus(mpPaymentId.ToString());

        // Buscar o pagamento local pelo ID do MP
        var payment = await _paymentRepository.GetByMercadoPagoPaymentId(mpPaymentId.ToString());

        var response = new ResponseDirectPaymentJson
        {
            Status = mpStatus,
            PaymentId = mpPaymentId
        };

        if (mpStatus == "approved" && payment != null)
        {
            // Atualizar pagamento local
            payment.Status = "approved";
            payment.PaidAt = DateTime.UtcNow;
            await _unitOfWork.Commit();

            // Upgrade user to ADMIN
            var userToUpdate = await _userUpdateOnlyRepository.GetById(payment.UserId);
            if (userToUpdate.Role != Roles.ADMIN)
            {
                userToUpdate.Role = Roles.ADMIN;
                _userUpdateOnlyRepository.Update(userToUpdate);
                await _unitOfWork.Commit();
            }

            // Gerar novo JWT
            response.Token = _tokenGenerator.Generate(userToUpdate);
            response.Name = userToUpdate.Name;
            response.Message = "Pagamento aprovado! Bem-vindo ao Laços de Carinho.";

            _logger.LogInformation("Pix payment {PaymentId} approved, user {UserId} upgraded to ADMIN",
                mpPaymentId, payment.UserId);
        }
        else if (mpStatus == "pending")
        {
            response.Message = "Aguardando pagamento do Pix...";
        }
        else if (mpStatus == "rejected")
        {
            response.Message = "Pagamento Pix rejeitado.";
        }
        else
        {
            response.Message = "Verificando status do pagamento...";
        }

        return response;
    }
}
