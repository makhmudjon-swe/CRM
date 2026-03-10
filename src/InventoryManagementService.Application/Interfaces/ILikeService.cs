namespace InventoryManagementService.Application.Interfaces;

public interface ILikeService
{
    Task<bool> ToggleAsync(int itemId, string userId);
    Task<int> GetCountAsync(int itemId);
    Task<bool> IsLikedByUserAsync(int itemId, string userId);
}
