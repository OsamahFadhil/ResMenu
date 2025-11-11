using MediatR;
using Menufy.Application.Common.Interfaces;
using Menufy.Application.Common.Models;
using Menufy.Application.Features.Auth.DTOs;
using Menufy.Domain.Entities;
using Menufy.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public RegisterCommandHandler(
        IApplicationDbContext context,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var dto = request.RegisterDto;

        // Check if email already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);

        if (existingUser != null)
        {
            return Result<AuthResponseDto>.FailureResult("Email already registered");
        }

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            PasswordHash = _passwordHasher.HashPassword(dto.Password),
            Role = UserRole.RestaurantOwner,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        // Create restaurant with slug
        var slug = GenerateSlug(dto.RestaurantName);
        var restaurant = new Restaurant
        {
            Id = Guid.NewGuid(),
            Name = dto.RestaurantName,
            Slug = slug,
            ContactPhone = dto.ContactPhone,
            OwnerId = user.Id,
            CreatedAt = DateTime.UtcNow
        };

        _context.Restaurants.Add(restaurant);

        await _context.SaveChangesAsync(cancellationToken);

        // Generate tokens
        var token = _jwtTokenService.GenerateToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Save refresh token to database
        var refreshTokenEntity = new Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

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
                RestaurantId = restaurant.Id
            }
        };

        return Result<AuthResponseDto>.SuccessResult(response, "Registration successful");
    }

    private static string GenerateSlug(string name)
    {
        var slug = name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and");

        // Add random suffix to ensure uniqueness
        slug += "-" + Guid.NewGuid().ToString("N").Substring(0, 6);

        return slug;
    }
}
