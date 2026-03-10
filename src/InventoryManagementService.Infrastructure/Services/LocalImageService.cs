using InventoryManagementService.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace InventoryManagementService.Infrastructure.Services;

public class LocalImageService : IImageService
{
    private readonly IWebHostEnvironment _env;
    private const string UploadFolder = "uploads";

    public LocalImageService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName)
    {
        var uploadsPath = Path.Combine(_env.WebRootPath, UploadFolder);
        Directory.CreateDirectory(uploadsPath);

        var ext = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(uploadsPath, uniqueName);

        using var fs = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fs);

        return $"/{UploadFolder}/{uniqueName}";
    }

    public Task DeleteAsync(string imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl)) return Task.CompletedTask;

        var relativePath = imageUrl.TrimStart('/');
        var fullPath = Path.Combine(_env.WebRootPath, relativePath);

        if (File.Exists(fullPath))
            File.Delete(fullPath);

        return Task.CompletedTask;
    }
}
