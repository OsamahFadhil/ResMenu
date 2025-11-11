using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;

namespace Menufy.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginDto LoginDto) : IRequest<Result<AuthResponseDto>>;
