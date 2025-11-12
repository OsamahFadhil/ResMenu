using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Queries.GetSettings;

public record GetRestaurantSettingsQuery(
    Guid RestaurantId,
    Guid UserId,
    string UserRole
) : IRequest<Result<RestaurantSettingsDto>>;

