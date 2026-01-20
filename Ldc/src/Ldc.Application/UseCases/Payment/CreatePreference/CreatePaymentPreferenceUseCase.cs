using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.Payment;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Domain.Services.Payment;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;
using Microsoft.Extensions.Configuration;

namespace Ldc.Application.UseCases.Payment.CreatePreference;

public class CreatePaymentPreferenceUseCase : ICreatePaymentPreferenceUseCase
{
    private readonly IMercadoPagoService _mercadoPagoService;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUserWriteOnlyRepository _userWriteOnlyRepository;
    private readonly IUserReadOnlyRepository _userReadOnlyRepository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILoggedUser _loggedUser;
    private readonly decimal _price;

    public CreatePaymentPreferenceUseCase(
        IMercadoPagoService mercadoPagoService,
        IPaymentRepository paymentRepository,
        IUserWriteOnlyRepository userWriteOnlyRepository,
        IUserReadOnlyRepository userReadOnlyRepository,
        IPasswordEncrypter passwordEncrypter,
        IUnitOfWork unitOfWork,
        ILoggedUser loggedUser,
        IConfiguration configuration)
    {
        _mercadoPagoService = mercadoPagoService;
        _paymentRepository = paymentRepository;
        _userWriteOnlyRepository = userWriteOnlyRepository;
        _userReadOnlyRepository = userReadOnlyRepository;
        _passwordEncrypter = passwordEncrypter;
        _unitOfWork = unitOfWork;
        _loggedUser = loggedUser;
        _price = configuration.GetValue<decimal>("Settings:MercadoPago:Price", 49.90m);
    }

    public async Task<ResponsePaymentPreferenceJson> Execute(RequestCreatePaymentJson request)
    {
        User user;
        string paymentType = request.PaymentType.ToLower();

        if (paymentType == "upgrade")
        {
            // Upgrade: usuário já está logado
            user = await _loggedUser.Get();
            
            if (user.Role == Domain.Enums.Roles.ADMIN)
            {
                throw new ErrorOnValidationException(["Você já possui uma conta de organizador."]);
            }
        }
        else
        {
            // Verifica se já existe usuário com este e-mail
            var existingUser = await _userReadOnlyRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
            {
                // Regra de segurança para sobrescrever conta pendente recente (tentativa de pagamento anterior falhou/desistiu)
                // 1. Não é ADMIN (não pagou plano)
                // 2. Criado há menos de 1 hora
                // Isso permite que o usuário tente se cadastrar novamente caso tenha tido problemas no pagamento anterior
                if (existingUser.Role == Domain.Enums.Roles.USER && 
                    existingUser.CreatedAt > DateTime.UtcNow.AddHours(-1))
                {
                    await _userWriteOnlyRepository.Delete(existingUser);
                    await _unitOfWork.Commit();
                }
            }

            // Novo cadastro: criar usuário pendente
            await ValidateNewAccount(request);
            
            user = new User
            {
                Name = request.Name!,
                Email = request.Email!,
                Password = _passwordEncrypter.Encrypt(request.Password!),
                UserIdentifier = Guid.NewGuid(),
                Role = Domain.Enums.Roles.USER // Começa como USER, vira ADMIN após pagamento
            };

            await _userWriteOnlyRepository.Add(user);
            await _unitOfWork.Commit();
        }

        // Criar referência interna
        var internalReference = Guid.NewGuid().ToString("N");

        // Criar preferência no Mercado Pago
        var (preferenceId, checkoutUrl) = await _mercadoPagoService.CreatePreference(
            title: "Laços de Carinho - Plano Organizador",
            description: "Acesso completo para criar e gerenciar suas listas de presentes",
            amount: _price,
            externalReference: internalReference,
            payerEmail: user.Email
        );

        // Salvar registro de pagamento
        // Nota: MercadoPagoPaymentId será preenchido quando recebermos o webhook do MP
        var payment = new Domain.Entities.Payment
        {
            UserId = user.Id,
            PreferenceId = internalReference,
            MercadoPagoPaymentId = null, // Será preenchido pelo webhook quando o pagamento for confirmado
            Status = "pending",
            Amount = _price,
            Description = "Plano Organizador - Laços de Carinho",
            PaymentType = paymentType
        };

        await _paymentRepository.Add(payment);
        await _unitOfWork.Commit();

        return new ResponsePaymentPreferenceJson
        {
            CheckoutUrl = checkoutUrl,
            PreferenceId = internalReference
        };
    }

    private async Task ValidateNewAccount(RequestCreatePaymentJson request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors.Add(ResourceErrorMessages.NAME_EMPTY);

        if (string.IsNullOrWhiteSpace(request.Email))
            errors.Add(ResourceErrorMessages.EMAIL_EMPTY);

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            errors.Add(ResourceErrorMessages.INVALID_PASSWORD);

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var emailExists = await _userReadOnlyRepository.ExistActiveUserWithEmail(request.Email);
            if (emailExists)
                errors.Add(ResourceErrorMessages.EMAIL_ALREADY_REGISTERED);
        }

        if (errors.Any())
            throw new ErrorOnValidationException(errors);
    }
}
