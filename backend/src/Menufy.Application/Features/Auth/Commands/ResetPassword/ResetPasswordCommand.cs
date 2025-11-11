using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommand : IRequest<Result>
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
