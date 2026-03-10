using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Name).IsRequired().HasMaxLength(300);
        builder.Property(i => i.ImageUrl).HasMaxLength(500);

        // Custom field names
        builder.Property(i => i.CustomString1Name).HasMaxLength(100);
        builder.Property(i => i.CustomString2Name).HasMaxLength(100);
        builder.Property(i => i.CustomString3Name).HasMaxLength(100);
        builder.Property(i => i.CustomText1Name).HasMaxLength(100);
        builder.Property(i => i.CustomText2Name).HasMaxLength(100);
        builder.Property(i => i.CustomText3Name).HasMaxLength(100);
        builder.Property(i => i.CustomInt1Name).HasMaxLength(100);
        builder.Property(i => i.CustomInt2Name).HasMaxLength(100);
        builder.Property(i => i.CustomInt3Name).HasMaxLength(100);
        builder.Property(i => i.CustomLink1Name).HasMaxLength(100);
        builder.Property(i => i.CustomLink2Name).HasMaxLength(100);
        builder.Property(i => i.CustomLink3Name).HasMaxLength(100);
        builder.Property(i => i.CustomBool1Name).HasMaxLength(100);
        builder.Property(i => i.CustomBool2Name).HasMaxLength(100);
        builder.Property(i => i.CustomBool3Name).HasMaxLength(100);

        builder.HasOne(i => i.Owner)
            .WithMany(u => u.OwnedInventories)
            .HasForeignKey(i => i.OwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Category)
            .WithMany(c => c.Inventories)
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.OwnerId);
        builder.HasIndex(i => i.CategoryId);
    }
}
