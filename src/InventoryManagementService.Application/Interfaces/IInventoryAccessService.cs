using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Domain.Enums;

namespace InventoryManagementService.Application.Interfaces;

public interface IInventoryAccessService
{
    Task<IEnumerable<InventoryAccessDto>> GetByInventoryAsync(int inventoryId);
    Task<IEnumerable<UserInventoryAccessDto>> GetByUserAsync(string userId);
    Task GrantAccessAsync(int inventoryId, string userId, AccessLevel level);
    Task RevokeAccessAsync(int inventoryId, string userId);
    Task SetPublicAsync(int inventoryId, bool isPublic);
}
