using InventoryManagementService.Application.DTOs;

namespace InventoryManagementService.Application.Interfaces;

public interface ICustomIdService
{
    Task<CustomIdFormatDto?> GetFormatAsync(int inventoryId);
    Task<CustomIdFormatDto> SaveFormatAsync(int inventoryId, SaveCustomIdFormatRequest request);
    Task DeleteFormatAsync(int inventoryId);
    Task<string> GenerateIdAsync(int inventoryId);
    string PreviewId(List<SaveCustomIdElementRequest> elements);
}
