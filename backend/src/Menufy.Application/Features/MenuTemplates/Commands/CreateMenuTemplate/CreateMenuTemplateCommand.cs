using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;

namespace Menufy.Application.Features.MenuTemplates.Commands.CreateMenuTemplate;

public record CreateMenuTemplateCommand(Guid UserId, string UserRole, UpsertMenuTemplateDto Template)
    : IRequest<Result<MenuTemplateDto>>;

