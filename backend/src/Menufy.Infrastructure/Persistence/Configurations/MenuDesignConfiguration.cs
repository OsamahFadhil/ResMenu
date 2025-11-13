using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menufy.Infrastructure.Persistence.Configurations;

public class MenuDesignConfiguration : IEntityTypeConfiguration<MenuDesign>
{
    public void Configure(EntityTypeBuilder<MenuDesign> builder)
    {
        builder.ToTable("MenuDesigns");

        builder.HasKey(md => md.Id);

        builder.Property(md => md.RestaurantId)
            .IsRequired();

        builder.Property(md => md.LayoutConfiguration)
            .IsRequired()
            .HasColumnType("jsonb"); // PostgreSQL

        builder.Property(md => md.GlobalTheme)
            .IsRequired()
            .HasColumnType("jsonb"); // PostgreSQL

        builder.Property(md => md.Version)
            .IsRequired()
            .HasDefaultValue(1);

        builder.Property(md => md.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(md => md.Name)
            .HasMaxLength(200);

        // Relationships
        builder.HasOne(md => md.Restaurant)
            .WithMany(r => r.MenuDesigns)
            .HasForeignKey(md => md.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(md => new { md.RestaurantId, md.IsActive })
            .HasDatabaseName("IX_MenuDesigns_RestaurantId_IsActive");

        builder.HasIndex(md => md.CreatedAt)
            .HasDatabaseName("IX_MenuDesigns_CreatedAt");
    }
}

