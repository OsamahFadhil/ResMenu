using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;

namespace Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplateById;

public record GetMenuTemplateByIdQuery(Guid Id, Guid UserId, string UserRole) : IRequest<Result<MenuTemplateDto>>;

