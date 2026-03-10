namespace InventoryManagementService.Application.Interfaces;

public interface IInventoryAuthorizationService
{
    Task<bool> CanViewInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator);
    Task<bool> CanEditInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage);
    Task<bool> CanDeleteInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage);
    Task<bool> CanManageAccessAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage);
    Task<bool> CanWriteItemsAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage);
}
