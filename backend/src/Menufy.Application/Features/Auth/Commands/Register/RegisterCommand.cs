using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;

namespace Menufy.Application.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterDto RegisterDto) : IRequest<Result<AuthResponseDto>>;
