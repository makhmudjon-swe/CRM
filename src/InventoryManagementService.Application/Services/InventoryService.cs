using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public InventoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<InventoryDto>> GetAllAsync()
    {
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    public async Task<IEnumerable<InventoryDto>> GetLatestAsync(int count)
    {
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .OrderByDescending(i => i.CreatedAt)
            .Take(count)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    public async Task<IEnumerable<InventoryDto>> GetTopPopularAsync(int count)
    {
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .OrderByDescending(i => i.Items.Count)
            .Take(count)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    public async Task<IEnumerable<InventoryDto>> GetByOwnerAsync(string ownerId)
    {
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .Where(i => i.OwnerId == ownerId)
            .OrderByDescending(i => i.UpdatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    public async Task<IEnumerable<InventoryDto>> GetAccessibleAsync(string userId)
    {
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .Include(i => i.AccessList)
            .Where(i => i.OwnerId == userId || i.IsPublic || i.AccessList.Any(a => a.UserId == userId))
            .OrderByDescending(i => i.UpdatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }

    public async Task<InventoryDto?> GetByIdAsync(int id)
    {
        var inventory = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .FirstOrDefaultAsync(i => i.Id == id);

        return inventory == null ? null : _mapper.Map<InventoryDto>(inventory);
    }

    public async Task<InventoryDto> CreateAsync(CreateInventoryDto dto, string ownerId)
    {
        var inventory = _mapper.Map<Inventory>(dto);
        inventory.OwnerId = ownerId;
        inventory.CreatedAt = DateTime.UtcNow;
        inventory.UpdatedAt = DateTime.UtcNow;

        await _uow.Inventories.AddAsync(inventory);
        await _uow.SaveChangesAsync();

        // Handle tags
        foreach (var tagName in dto.Tags)
        {
            var tag = (await _uow.Tags.FindAsync(t => t.Name == tagName)).FirstOrDefault();
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                await _uow.Tags.AddAsync(tag);
                await _uow.SaveChangesAsync();
            }
            await _uow.InventoryTags.AddAsync(new InventoryTag { InventoryId = inventory.Id, TagId = tag.Id });
        }
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(inventory.Id))!;
    }

    public async Task<InventoryDto> UpdateAsync(UpdateInventoryDto dto)
    {
        var inventory = await _uow.Inventories.Query()
            .Include(i => i.InventoryTags)
            .FirstOrDefaultAsync(i => i.Id == dto.Id)
            ?? throw new KeyNotFoundException($"Inventory {dto.Id} not found");

        _mapper.Map(dto, inventory);
        inventory.UpdatedAt = DateTime.UtcNow;

        // Sync tags
        _uow.InventoryTags.RemoveRange(inventory.InventoryTags);
        await _uow.SaveChangesAsync();

        foreach (var tagName in dto.Tags)
        {
            var tag = (await _uow.Tags.FindAsync(t => t.Name == tagName)).FirstOrDefault();
            if (tag == null)
            {
                tag = new Tag { Name = tagName };
                await _uow.Tags.AddAsync(tag);
                await _uow.SaveChangesAsync();
            }
            await _uow.InventoryTags.AddAsync(new InventoryTag { InventoryId = inventory.Id, TagId = tag.Id });
        }
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(inventory.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var inventory = await _uow.Inventories.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Inventory {id} not found");
        _uow.Inventories.Remove(inventory);
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryDto>> SearchAsync(string query)
    {
        var lowerQuery = query.ToLower();
        var inventories = await _uow.Inventories.Query()
            .Include(i => i.Category)
            .Include(i => i.Owner)
            .Include(i => i.Items)
            .Include(i => i.InventoryTags).ThenInclude(it => it.Tag)
            .Where(i => i.Name.ToLower().Contains(lowerQuery) ||
                        (i.Description != null && i.Description.ToLower().Contains(lowerQuery)))
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return _mapper.Map<IEnumerable<InventoryDto>>(inventories);
    }
}
