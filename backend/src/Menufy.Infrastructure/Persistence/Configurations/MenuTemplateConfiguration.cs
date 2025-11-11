using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menufy.Infrastructure.Persistence.Configurations;

public class MenuTemplateConfiguration : IEntityTypeConfiguration<MenuTemplate>
{
    public void Configure(EntityTypeBuilder<MenuTemplate> builder)
    {
        builder.ToTable("MenuTemplates");

        builder.HasKey(mt => mt.Id);

        builder.Property(mt => mt.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(mt => mt.Description)
            .HasMaxLength(500);

        builder.Property(mt => mt.Structure)
            .IsRequired()
            .HasColumnType("jsonb");

        builder.Property(mt => mt.Theme)
            .HasColumnType("jsonb");

        builder.Property(mt => mt.Tags)
            .HasColumnType("jsonb");

        builder.HasOne(mt => mt.Restaurant)
            .WithMany(r => r.MenuTemplates)
            .HasForeignKey(mt => mt.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mt => new { mt.RestaurantId, mt.Name }).IsUnique();
    }
}

