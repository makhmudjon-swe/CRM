using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Enums;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class InventoryAccessService : IInventoryAccessService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryAccessService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<InventoryAccessDto>> GetByInventoryAsync(int inventoryId)
    {
        var accessList = await _unitOfWork.InventoryAccesses.Query()
            .Include(a => a.User)
            .Where(a => a.InventoryId == inventoryId)
            .ToListAsync();

        return accessList.Select(a => new InventoryAccessDto
        {
            Id = a.Id,
            UserId = a.UserId,
            UserDisplayName = a.User.DisplayName,
            UserEmail = a.User.Email,
            AccessLevel = a.AccessLevel,
            GrantedAt = a.GrantedAt
        });
    }

    public async Task<IEnumerable<UserInventoryAccessDto>> GetByUserAsync(string userId)
    {
        var accessList = await _unitOfWork.InventoryAccesses.Query()
            .Include(a => a.Inventory).ThenInclude(i => i.Owner)
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return accessList.Select(a => new UserInventoryAccessDto
        {
            InventoryId = a.InventoryId,
            InventoryName = a.Inventory.Name,
            OwnerName = a.Inventory.Owner.DisplayName ?? a.Inventory.Owner.Email,
            AccessLevel = a.AccessLevel,
            GrantedAt = a.GrantedAt
        });
    }

    public async Task GrantAccessAsync(int inventoryId, string userId, AccessLevel level)
    {
        var existing = (await _unitOfWork.InventoryAccesses.FindAsync(
            a => a.InventoryId == inventoryId && a.UserId == userId)).FirstOrDefault();

        if (existing != null)
        {
            existing.AccessLevel = level;
            _unitOfWork.InventoryAccesses.Update(existing);
        }
        else
        {
            await _unitOfWork.InventoryAccesses.AddAsync(new InventoryAccess
            {
                InventoryId = inventoryId,
                UserId = userId,
                AccessLevel = level,
                GrantedAt = DateTime.UtcNow
            });
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RevokeAccessAsync(int inventoryId, string userId)
    {
        var existing = (await _unitOfWork.InventoryAccesses.FindAsync(
            a => a.InventoryId == inventoryId && a.UserId == userId)).FirstOrDefault();

        if (existing != null)
        {
            _unitOfWork.InventoryAccesses.Remove(existing);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task SetPublicAsync(int inventoryId, bool isPublic)
    {
        var inventory = await _unitOfWork.Inventories.GetByIdAsync(inventoryId);
        if (inventory != null)
        {
            inventory.IsPublic = isPublic;
            _unitOfWork.Inventories.Update(inventory);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
