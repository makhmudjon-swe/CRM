using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class CustomIdElementConfiguration : IEntityTypeConfiguration<CustomIdElement>
{
    public void Configure(EntityTypeBuilder<CustomIdElement> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Value).HasMaxLength(100);
        builder.Property(e => e.DateFormat).HasMaxLength(50);

        builder.HasOne(e => e.CustomIdFormat)
            .WithMany(f => f.Elements)
            .HasForeignKey(e => e.CustomIdFormatId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => new { e.CustomIdFormatId, e.SortOrder });
    }
}
