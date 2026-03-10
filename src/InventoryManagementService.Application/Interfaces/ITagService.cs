using InventoryManagementService.Application.DTOs;

namespace InventoryManagementService.Application.Interfaces;

public interface ITagService
{
    Task<IEnumerable<TagDto>> GetAllAsync();
    Task<IEnumerable<TagDto>> GetPopularAsync(int count);
    Task<IEnumerable<TagDto>> SearchAsync(string query);
}
