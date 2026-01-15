using AutoMapper;
using Ldc.Communication.Requests;
using Ldc.Communication.Responses;
using Ldc.Domain.Entities;

namespace Ldc.Application.AutoMapper;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        RequestToEntity();
        EntityToResponse();
    }
    
    private void RequestToEntity()
    {
        CreateMap<RequestRegisterUserJson, User>()
            .ForMember(dest => dest.Password, config => config.Ignore())
            .ForMember(dest => dest.Role, config => config.MapFrom(src => src.Role.ToLower()));
        
        CreateMap<RequestExpenseJson, Expense>().ForMember(dest => dest.Tags, config => config.MapFrom(source => source.Tags.Distinct()));
        
        CreateMap<Ldc.Communication.Enums.Tag, Tag>().ForMember(dest => dest.Value, config => config.MapFrom(source => source));
        
    }
    
    private void EntityToResponse()
    {
        CreateMap<Expense, ResponseExpenseJson>()
            .ForMember(dest => dest.Tags, config => config.MapFrom(source => source.Tags.Select(tag => tag.Value)));
        
        CreateMap<Expense, ResponseRegisterExpenseJson>();
        CreateMap<Expense, ResponseShortExpenseJson>();
        CreateMap<User, ResponseUserProfileJson>();
    }
}