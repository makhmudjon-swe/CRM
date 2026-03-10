namespace InventoryManagementService.Application.Interfaces;

public interface IImageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName);
    Task DeleteAsync(string imageUrl);
}
