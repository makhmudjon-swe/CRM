using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesaleCRM.Domain.Entities;

namespace WholesaleCRM.Infrastructure.Data.Configurations;

public class DealConfiguration : IEntityTypeConfiguration<Deal>
{
    public void Configure(EntityTypeBuilder<Deal> builder)
    {
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Title).IsRequired().HasMaxLength(200);
        builder.Property(d => d.Status).HasConversion<int>();
        builder.Property(d => d.TotalAmount).HasPrecision(18, 2);

        builder.HasOne(d => d.Customer)
            .WithMany(c => c.Deals)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.AssignedTo)
            .WithMany(u => u.AssignedDeals)
            .HasForeignKey(d => d.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
