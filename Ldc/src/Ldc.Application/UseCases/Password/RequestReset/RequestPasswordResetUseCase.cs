using Ldc.Communication.Requests;
using Ldc.Domain.Entities;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.PasswordReset;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Services.Email;

namespace Ldc.Application.UseCases.Password.RequestReset;

public class RequestPasswordResetUseCase : IRequestPasswordResetUseCase
{
    private readonly IUserReadOnlyRepository _userRepository;
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IEmailService _emailService;
    private readonly IUnitOfWork _unitOfWork;

    public RequestPasswordResetUseCase(
        IUserReadOnlyRepository userRepository,
        IPasswordResetTokenRepository tokenRepository,
        IEmailService emailService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _emailService = emailService;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(RequestPasswordResetJson request, string baseUrl)
    {
        var user = await _userRepository.GetUserByEmail(request.Email);
        
        // Não revelamos se o email existe ou não por segurança
        if (user is null)
        {
            // Silently return - don't reveal if email exists
            return;
        }

        // Invalida tokens anteriores
        await _tokenRepository.InvalidateAllUserTokens(user.Id);

        // Gera novo token
        var token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        var resetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            IsUsed = false
        };

        await _tokenRepository.Add(resetToken);
        await _unitOfWork.Commit();

        // Monta o link de reset
        var resetLink = $"{baseUrl}/reset-password?token={token}";

        // Envia email
        await _emailService.SendPasswordResetEmailAsync(user.Email, user.Name, resetLink);
    }
}
