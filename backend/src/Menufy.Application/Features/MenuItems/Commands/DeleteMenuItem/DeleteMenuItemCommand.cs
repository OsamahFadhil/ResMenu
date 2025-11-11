using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.MenuItems.Commands.DeleteMenuItem;

public class DeleteMenuItemCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
