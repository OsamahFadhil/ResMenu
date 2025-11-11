using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;

namespace Menufy.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
