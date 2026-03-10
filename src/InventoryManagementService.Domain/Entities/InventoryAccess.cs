using InventoryManagementService.Domain.Enums;

namespace InventoryManagementService.Domain.Entities;

public class InventoryAccess
{
    public int Id { get; set; }

    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public AccessLevel AccessLevel { get; set; } = AccessLevel.ReadOnly;
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
}
