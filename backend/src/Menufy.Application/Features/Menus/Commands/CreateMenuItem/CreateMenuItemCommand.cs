using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Menus.DTOs;

namespace Menufy.Application.Features.Menus.Commands.CreateMenuItem;

public record CreateMenuItemCommand(Guid CategoryId, CreateMenuItemDto Dto) : IRequest<Result<MenuItemDto>>;
