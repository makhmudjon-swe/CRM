using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class CustomIdFormatConfiguration : IEntityTypeConfiguration<CustomIdFormat>
{
    public void Configure(EntityTypeBuilder<CustomIdFormat> builder)
    {
        builder.HasKey(f => f.Id);

        builder.HasOne(f => f.Inventory)
            .WithMany(i => i.CustomIdFormats)
            .HasForeignKey(f => f.InventoryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
