using System.Reflection;
using Menufy.Application.Common.Interfaces;
using Menufy.Domain.Common;
using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Menufy.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MenuCategory> MenuCategories => Set<MenuCategory>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuTemplate> MenuTemplates => Set<MenuTemplate>();
    public DbSet<MenuDesign> MenuDesigns => Set<MenuDesign>();
    public DbSet<QRCode> QRCodes => Set<QRCode>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global query filter for soft delete
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        modelBuilder.Entity<Restaurant>().HasQueryFilter(r => !r.IsDeleted);
        modelBuilder.Entity<MenuCategory>().HasQueryFilter(mc => !mc.IsDeleted);
        modelBuilder.Entity<MenuItem>().HasQueryFilter(mi => !mi.IsDeleted);
        modelBuilder.Entity<MenuTemplate>().HasQueryFilter(mt => !mt.IsDeleted);
        modelBuilder.Entity<MenuDesign>().HasQueryFilter(md => !md.IsDeleted);
        modelBuilder.Entity<QRCode>().HasQueryFilter(q => !q.IsDeleted);
        modelBuilder.Entity<RefreshToken>().HasQueryFilter(rt => !rt.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Implement soft delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
