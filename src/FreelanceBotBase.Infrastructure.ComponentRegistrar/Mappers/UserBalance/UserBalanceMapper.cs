using AutoMapper;
using FreelanceBotBase.Contracts.UserBalance;

namespace FreelanceBotBase.Infrastructure.ComponentRegistrar.Mappers.UserBalance
{
    public class UserBalanceMapper : Profile
    {
        public UserBalanceMapper()
        {
            CreateMap<Domain.UserBalance.UserBalance, UserBalanceDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance));
        }
    }
}
