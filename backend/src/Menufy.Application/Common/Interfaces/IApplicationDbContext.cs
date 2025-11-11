using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Restaurant> Restaurants { get; }
    DbSet<MenuCategory> MenuCategories { get; }
    DbSet<MenuItem> MenuItems { get; }
    DbSet<MenuTemplate> MenuTemplates { get; }
    DbSet<QRCode> QRCodes { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
