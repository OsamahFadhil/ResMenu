using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.MenuTemplates.DTOs;

namespace Menufy.Application.Features.MenuTemplates.Queries.GetMenuTemplates;

public record GetMenuTemplatesQuery(Guid UserId, string UserRole) : IRequest<Result<List<MenuTemplateDto>>>;

