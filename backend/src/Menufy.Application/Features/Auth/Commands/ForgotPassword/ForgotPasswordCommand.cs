using MediatR;
using Menufy.Application.Common.Models;

namespace Menufy.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommand : IRequest<Result>
{
    public string Email { get; set; } = string.Empty;
}
