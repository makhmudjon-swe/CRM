using Microsoft.AspNetCore.Identity;

namespace InventoryManagementService.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public string PreferredTheme { get; set; } = "light";
    public string PreferredLanguage { get; set; } = "en";
    public bool CanManageInventories { get; set; }

    public ICollection<Inventory> OwnedInventories { get; set; } = new List<Inventory>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<InventoryAccess> InventoryAccesses { get; set; } = new List<InventoryAccess>();
}
