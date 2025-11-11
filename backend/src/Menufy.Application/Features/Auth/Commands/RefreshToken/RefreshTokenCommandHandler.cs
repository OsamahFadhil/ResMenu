using MediatR;
using Menufy.Application.Common.Models;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Features.Auth.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtTokenService jwtTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken, cancellationToken);

        if (refreshToken == null)
            return Result<AuthResponseDto>.FailureResult("Invalid refresh token");

        if (!refreshToken.IsActive)
            return Result<AuthResponseDto>.FailureResult("Refresh token is expired or revoked");

        // Generate new tokens
        var newAccessToken = _jwtTokenService.GenerateToken(
            refreshToken.User.Id,
            refreshToken.User.Email,
            refreshToken.User.Name,
            refreshToken.User.Role.ToString()
        );

        var newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // Revoke old refresh token
        refreshToken.IsRevoked = true;

        // Create new refresh token
        var newRefreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            UserId = refreshToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        var response = new AuthResponseDto
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken,
            User = new UserDto
            {
                Id = refreshToken.User.Id,
                Name = refreshToken.User.Name,
                Email = refreshToken.User.Email,
                Role = refreshToken.User.Role.ToString()
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response);
    }
}
