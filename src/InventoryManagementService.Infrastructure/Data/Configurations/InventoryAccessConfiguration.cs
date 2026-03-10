using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class InventoryAccessConfiguration : IEntityTypeConfiguration<InventoryAccess>
{
    public void Configure(EntityTypeBuilder<InventoryAccess> builder)
    {
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.InventoryId, a.UserId }).IsUnique();

        builder.HasOne(a => a.Inventory)
            .WithMany(i => i.AccessList)
            .HasForeignKey(a => a.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.User)
            .WithMany(u => u.InventoryAccesses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
