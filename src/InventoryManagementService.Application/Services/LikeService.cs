using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class LikeService : ILikeService
{
    private readonly IUnitOfWork _uow;

    public LikeService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> ToggleAsync(int itemId, string userId)
    {
        var existing = await _uow.Likes.Query()
            .FirstOrDefaultAsync(l => l.ItemId == itemId && l.UserId == userId);

        if (existing != null)
        {
            _uow.Likes.Remove(existing);
            await _uow.SaveChangesAsync();
            return false;
        }

        await _uow.Likes.AddAsync(new Like { ItemId = itemId, UserId = userId });
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetCountAsync(int itemId)
    {
        return await _uow.Likes.Query().CountAsync(l => l.ItemId == itemId);
    }

    public async Task<bool> IsLikedByUserAsync(int itemId, string userId)
    {
        return await _uow.Likes.Query().AnyAsync(l => l.ItemId == itemId && l.UserId == userId);
    }
}
