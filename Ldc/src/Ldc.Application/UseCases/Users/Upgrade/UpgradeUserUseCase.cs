using Ldc.Communication.Responses;
using Ldc.Domain.Enums;
using Ldc.Domain.Repositories;
using Ldc.Domain.Repositories.User;
using Ldc.Domain.Security.Tokens;
using Ldc.Domain.Services.LoggedUser;
using Ldc.Exception;
using Ldc.Exception.ExceptionBase;

namespace Ldc.Application.UseCases.Users.Upgrade;

public class UpgradeUserUseCase : IUpgradeUserUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IUserUpdateOnlyRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAccessTokenGenerator _tokenGenerator;

    public UpgradeUserUseCase(
        ILoggedUser loggedUser,
        IUserUpdateOnlyRepository userRepository,
        IUnitOfWork unitOfWork,
        IAccessTokenGenerator tokenGenerator)
    {
        _loggedUser = loggedUser;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<ResponseUpgradeUserJson> Execute()
    {
        var user = await _loggedUser.Get();

        // Verifica se já é admin
        if (user.Role == Roles.ADMIN)
        {
            throw new ErrorOnValidationException(["Você já possui uma conta de casal."]);
        }

        // Faz upgrade para admin
        var userToUpdate = await _userRepository.GetById(user.Id);
        userToUpdate.Role = Roles.ADMIN;
        
        _userRepository.Update(userToUpdate);
        await _unitOfWork.Commit();

        // Gera novo token com role atualizada
        var newToken = _tokenGenerator.Generate(userToUpdate);

        return new ResponseUpgradeUserJson
        {
            Token = newToken,
            Message = "Sua conta foi atualizada com sucesso! Agora você pode criar sua lista de presentes."
        };
    }
}
