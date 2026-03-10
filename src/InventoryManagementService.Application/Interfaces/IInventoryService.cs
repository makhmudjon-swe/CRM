using InventoryManagementService.Application.DTOs;

namespace InventoryManagementService.Application.Interfaces;

public interface IInventoryService
{
    Task<IEnumerable<InventoryDto>> GetAllAsync();
    Task<IEnumerable<InventoryDto>> GetLatestAsync(int count);
    Task<IEnumerable<InventoryDto>> GetTopPopularAsync(int count);
    Task<IEnumerable<InventoryDto>> GetByOwnerAsync(string ownerId);
    Task<IEnumerable<InventoryDto>> GetAccessibleAsync(string userId);
    Task<InventoryDto?> GetByIdAsync(int id);
    Task<InventoryDto> CreateAsync(CreateInventoryDto dto, string ownerId);
    Task<InventoryDto> UpdateAsync(UpdateInventoryDto dto);
    Task DeleteAsync(int id);
    Task<IEnumerable<InventoryDto>> SearchAsync(string query);
}
