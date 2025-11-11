using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menufy.Infrastructure.Persistence.Configurations;

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>
{
    public void Configure(EntityTypeBuilder<Restaurant> builder)
    {
        builder.ToTable("Restaurants");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(r => r.Slug)
            .IsUnique();

        builder.Property(r => r.LogoUrl)
            .HasMaxLength(500);

        builder.Property(r => r.ContactPhone)
            .HasMaxLength(20);

        builder.Property(r => r.ContactEmail)
            .HasMaxLength(200);

        builder.Property(r => r.Address)
            .HasMaxLength(500);

        builder.HasMany(r => r.MenuCategories)
            .WithOne(c => c.Restaurant)
            .HasForeignKey(c => c.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.QRCode)
            .WithOne(q => q.Restaurant)
            .HasForeignKey<QRCode>(q => q.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
