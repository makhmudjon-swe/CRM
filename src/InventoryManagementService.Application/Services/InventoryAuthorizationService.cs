using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Enums;
using InventoryManagementService.Domain.Interfaces;

namespace InventoryManagementService.Application.Services;

public class InventoryAuthorizationService : IInventoryAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryAuthorizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> CanViewInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator)
    {
        if (isAdmin || isCreator) return true;

        var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
        if (inventory == null) return false;

        // Owner can always view their own
        if (inventory.OwnerId == userId) return true;

        // Public inventories are visible to all authenticated users
        if (inventory.IsPublic) return true;

        // Check if user has any access grant
        var access = await _unitOfWork.InventoryAccesses.FindAsync(
            a => a.InventoryId == inventoryId && a.UserId == userId);
        return access.Any();
    }

    public async Task<bool> CanEditInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage)
    {
        if (isAdmin || isCreator) return true;
        if (!canManage) return false;

        var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
        return inventory != null && inventory.OwnerId == userId;
    }

    public async Task<bool> CanDeleteInventoryAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage)
    {
        return await CanEditInventoryAsync(inventoryId, userId, isAdmin, isCreator, canManage);
    }

    public async Task<bool> CanManageAccessAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage)
    {
        return await CanEditInventoryAsync(inventoryId, userId, isAdmin, isCreator, canManage);
    }

    public async Task<bool> CanWriteItemsAsync(int inventoryId, string userId, bool isAdmin, bool isCreator, bool canManage)
    {
        if (isAdmin || isCreator) return true;

        var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
        if (inventory == null) return false;

        // Owner can write only if they have manage permission
        if (inventory.OwnerId == userId && canManage) return true;

        // Check if user has explicit write access grant
        var access = await _unitOfWork.InventoryAccesses.FindAsync(
            a => a.InventoryId == inventoryId && a.UserId == userId && a.AccessLevel >= AccessLevel.ReadWrite);
        return access.Any();
    }
}
