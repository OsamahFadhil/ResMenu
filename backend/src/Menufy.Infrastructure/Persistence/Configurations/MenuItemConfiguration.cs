using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menufy.Infrastructure.Persistence.Configurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Description)
            .HasMaxLength(1000);

        builder.Property(i => i.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(500);

        builder.Property(i => i.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(i => i.DisplayOrder)
            .IsRequired();
    }
}
