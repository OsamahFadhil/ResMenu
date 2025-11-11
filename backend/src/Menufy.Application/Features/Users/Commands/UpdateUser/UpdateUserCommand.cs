using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}
