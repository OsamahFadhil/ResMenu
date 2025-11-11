using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Restaurants.DTOs;

namespace Menufy.Application.Features.Restaurants.Commands.UpdateRestaurant;

public record UpdateRestaurantCommand(Guid RestaurantId, Guid UserId, UpdateRestaurantDto Dto) : IRequest<Result<RestaurantDto>>;
