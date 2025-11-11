using AutoMapper;
using Menufy.Application.Features.Auth.DTOs;
using Menufy.Application.Features.Restaurants.Common;
using Menufy.Domain.Entities;

namespace Menufy.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()));

        CreateMap<Restaurant, RestaurantDto>()
            .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Owner.Name));
    }
}
