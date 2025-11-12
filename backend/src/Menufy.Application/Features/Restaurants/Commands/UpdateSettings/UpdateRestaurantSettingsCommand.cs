using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateSettings;

public record UpdateRestaurantSettingsCommand(
    Guid RestaurantId,
    Guid UserId,
    string UserRole,
    UpdateRestaurantSettingsDto Settings
) : IRequest<Result<RestaurantSettingsDto>>;

