using Ldc.Communication.Requests;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.PasswordReset;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Cryptography;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Password.ResetPassword;

public class ResetPasswordUseCase : IResetPasswordUseCase
{
    private readonly IPasswordResetTokenRepository _tokenRepository;
    private readonly IUserUpdateOnlyRepository _userRepository;
    private readonly IPasswordEncrypter _passwordEncrypter;
    private readonly IUnitOfWork _unitOfWork;

    public ResetPasswordUseCase(
        IPasswordResetTokenRepository tokenRepository,
        IUserUpdateOnlyRepository userRepository,
        IPasswordEncrypter passwordEncrypter,
        IUnitOfWork unitOfWork)
    {
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _passwordEncrypter = passwordEncrypter;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(ResetPasswordJson request)
    {
        // Validação básica
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            throw new ErrorOnValidationException(["Token inválido"]);
        }

        if (string.IsNullOrWhiteSpace(request.NewPassword) || request.NewPassword.Length < 6)
        {
            throw new ErrorOnValidationException(["A senha deve ter pelo menos 6 caracteres"]);
        }

        // Busca e valida o token
        var resetToken = await _tokenRepository.GetByToken(request.Token);
        if (resetToken is null)
        {
            throw new ErrorOnValidationException(["Token inválido ou expirado"]);
        }

        // Busca o usuário
        var user = await _userRepository.GetById(resetToken.UserId);

        // Atualiza a senha
        user.Password = _passwordEncrypter.Encrypt(request.NewPassword);
        _userRepository.Update(user);

        // Marca o token como usado
        resetToken.IsUsed = true;

        // Invalida outros tokens do usuário
        await _tokenRepository.InvalidateAllUserTokens(user.Id);

        await _unitOfWork.Commit();
    }
}
