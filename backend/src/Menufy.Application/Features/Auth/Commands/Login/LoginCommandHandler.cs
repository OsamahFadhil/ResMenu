using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var dto = request.LoginDto;

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
        {
            return Result<AuthResponseDto>.FailureResult("Invalid email or password");
        }

        var restaurant = await _context.Restaurants
            .FirstOrDefaultAsync(r => r.OwnerId == user.Id, cancellationToken);

        var token = _jwtTokenService.GenerateToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        var response = new AuthResponseDto
        {
            Token = token,
            RefreshToken = refreshToken,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.ToString(),
                RestaurantId = restaurant?.Id
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Login successful");
    }
}
