using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuTemplates.Commands.DeleteMenuTemplate;

public record DeleteMenuTemplateCommand(Guid Id, Guid UserId, string UserRole) : IRequest<Result>;

