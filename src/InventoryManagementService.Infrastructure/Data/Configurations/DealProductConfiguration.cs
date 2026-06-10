using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WholesaleCRM.Domain.Entities;

namespace WholesaleCRM.Infrastructure.Data.Configurations;

public class DealProductConfiguration : IEntityTypeConfiguration<DealProduct>
{
    public void Configure(EntityTypeBuilder<DealProduct> builder)
    {
        builder.HasKey(dp => dp.Id);
        builder.Property(dp => dp.UnitPrice).HasPrecision(18, 2);
        builder.Property(dp => dp.Discount).HasPrecision(5, 2);
        builder.Ignore(dp => dp.TotalPrice);

        builder.HasOne(dp => dp.Deal)
            .WithMany(d => d.DealProducts)
            .HasForeignKey(dp => dp.DealId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(dp => dp.Product)
            .WithMany(p => p.DealProducts)
            .HasForeignKey(dp => dp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
