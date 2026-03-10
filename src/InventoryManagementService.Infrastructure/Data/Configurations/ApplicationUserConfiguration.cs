using InventoryManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagementService.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.DisplayName).HasMaxLength(200);
        builder.Property(u => u.AvatarUrl).HasMaxLength(500);
        builder.Property(u => u.PreferredTheme).HasMaxLength(10).HasDefaultValue("light");
        builder.Property(u => u.PreferredLanguage).HasMaxLength(10).HasDefaultValue("en");
    }
}
