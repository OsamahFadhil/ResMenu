using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Commands.CreateCategory;

public record CreateCategoryCommand(Guid RestaurantId, CreateCategoryDto Dto) : IRequest<Result<MenuCategoryDto>>;
