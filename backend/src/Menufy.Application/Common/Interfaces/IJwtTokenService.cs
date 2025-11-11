using Menufy.Domain.Entities;

namespace Menufy.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
}
