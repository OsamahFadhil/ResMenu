using Menufy.Domain.Entities;

namespace Menufy.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateToken(Guid userId, string email, string name, string role);
    string GenerateRefreshToken();
}
