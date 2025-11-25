using AutoMapper;
using Ldc.Communication.Responses;
using Ldc.Domain.Services.LoggedUser;

namespace Ldc.Application.UseCases.Users.Profile;

public class GetUserProfileUseCase : IGetUserProfileUseCase
{
    private readonly ILoggedUser _loggedUser;
    private readonly IMapper _mapper;

    public GetUserProfileUseCase(ILoggedUser loggedUser, IMapper mapper)
    {
        _loggedUser = loggedUser;
        _mapper = mapper;
    }
    public async Task<ResponseUserProfileJson> Execute()
    {
        var user = await _loggedUser.Get();
        return _mapper.Map<ResponseUserProfileJson>(user);
    }
}