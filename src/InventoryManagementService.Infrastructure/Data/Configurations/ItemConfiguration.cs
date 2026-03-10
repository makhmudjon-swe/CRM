using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class ItemConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(300);
        builder.Property(i => i.CustomId).HasMaxLength(100);

        builder.Property(i => i.CustomString1Value).HasMaxLength(500);
        builder.Property(i => i.CustomString2Value).HasMaxLength(500);
        builder.Property(i => i.CustomString3Value).HasMaxLength(500);
        builder.Property(i => i.CustomLink1Value).HasMaxLength(500);
        builder.Property(i => i.CustomLink2Value).HasMaxLength(500);
        builder.Property(i => i.CustomLink3Value).HasMaxLength(500);

        // Composite unique index on InventoryId + CustomId
        builder.HasIndex(i => new { i.InventoryId, i.CustomId })
            .IsUnique()
            .HasFilter("\"CustomId\" IS NOT NULL");

        builder.HasOne(i => i.Inventory)
            .WithMany(inv => inv.Items)
            .HasForeignKey(i => i.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.InventoryId);
    }
}
