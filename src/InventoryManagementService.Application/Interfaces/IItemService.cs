using InventoryManagementService.Application.DTOs;

namespace InventoryManagementService.Application.Interfaces;

public interface IItemService
{
    Task<IEnumerable<ItemDto>> GetByInventoryAsync(int inventoryId, string? currentUserId = null);
    Task<ItemDto?> GetByIdAsync(int id, string? currentUserId = null);
    Task<ItemDto> CreateAsync(CreateItemDto dto);
    Task<ItemDto> UpdateAsync(UpdateItemDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<ItemDto>> SearchAsync(string query, string? currentUserId = null);
}
