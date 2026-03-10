using InventoryManagementService.Domain.Enums;

namespace InventoryManagementService.Application.DTOs;

public class InventoryAccessDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string? UserDisplayName { get; set; }
    public string? UserEmail { get; set; }
    public AccessLevel AccessLevel { get; set; }
    public DateTime GrantedAt { get; set; }
}

public class UserInventoryAccessDto
{
    public int InventoryId { get; set; }
    public string InventoryName { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public AccessLevel AccessLevel { get; set; }
    public DateTime GrantedAt { get; set; }
}
