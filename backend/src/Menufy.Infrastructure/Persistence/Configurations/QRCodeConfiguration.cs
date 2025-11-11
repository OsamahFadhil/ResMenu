using Menufy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Menufy.Infrastructure.Persistence.Configurations;

public class QRCodeConfiguration : IEntityTypeConfiguration<QRCode>
{
    public void Configure(EntityTypeBuilder<QRCode> builder)
    {
        builder.ToTable("QRCodes");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.ImageUrl)
            .IsRequired();

        builder.Property(q => q.Link)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(q => q.RestaurantId)
            .IsUnique();
    }
}
