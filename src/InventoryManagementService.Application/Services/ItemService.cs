using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class ItemService : IItemService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly ICustomIdService _customIdService;

    public ItemService(IUnitOfWork uow, IMapper mapper, ICustomIdService customIdService)
    {
        _uow = uow;
        _mapper = mapper;
        _customIdService = customIdService;
    }

    public async Task<IEnumerable<ItemDto>> GetByInventoryAsync(int inventoryId, string? currentUserId = null)
    {
        var items = await _uow.Items.Query()
            .Include(i => i.Likes)
            .Where(i => i.InventoryId == inventoryId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<ItemDto>>(items);
        if (currentUserId != null)
        {
            foreach (var dto in dtos)
            {
                dto.IsLikedByCurrentUser = items.First(i => i.Id == dto.Id)
                    .Likes.Any(l => l.UserId == currentUserId);
            }
        }
        return dtos;
    }

    public async Task<ItemDto?> GetByIdAsync(int id, string? currentUserId = null)
    {
        var item = await _uow.Items.Query()
            .Include(i => i.Likes)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null) return null;

        var dto = _mapper.Map<ItemDto>(item);
        if (currentUserId != null)
            dto.IsLikedByCurrentUser = item.Likes.Any(l => l.UserId == currentUserId);
        return dto;
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto dto)
    {
        var item = _mapper.Map<Item>(dto);
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;

        // Generate custom ID if format is configured
        var customId = await _customIdService.GenerateIdAsync(dto.InventoryId);
        if (!string.IsNullOrEmpty(customId))
            item.CustomId = customId;

        await _uow.Items.AddAsync(item);
        await _uow.SaveChangesAsync();

        return (await GetByIdAsync(item.Id))!;
    }

    public async Task<ItemDto> UpdateAsync(UpdateItemDto dto)
    {
        var item = await _uow.Items.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Item {dto.Id} not found");

        _mapper.Map(dto, item);
        item.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync();
        return (await GetByIdAsync(item.Id))!;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _uow.Items.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Item {id} not found");
        _uow.Items.Remove(item);
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<ItemDto>> SearchAsync(string query, string? currentUserId = null)
    {
        var lowerQuery = query.ToLower();
        var items = await _uow.Items.Query()
            .Include(i => i.Likes)
            .Where(i => i.Name.ToLower().Contains(lowerQuery) ||
                        (i.CustomId != null && i.CustomId.ToLower().Contains(lowerQuery)) ||
                        (i.CustomString1Value != null && i.CustomString1Value.ToLower().Contains(lowerQuery)) ||
                        (i.CustomString2Value != null && i.CustomString2Value.ToLower().Contains(lowerQuery)) ||
                        (i.CustomString3Value != null && i.CustomString3Value.ToLower().Contains(lowerQuery)) ||
                        (i.CustomText1Value != null && i.CustomText1Value.ToLower().Contains(lowerQuery)) ||
                        (i.CustomText2Value != null && i.CustomText2Value.ToLower().Contains(lowerQuery)) ||
                        (i.CustomText3Value != null && i.CustomText3Value.ToLower().Contains(lowerQuery)))
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        var dtos = _mapper.Map<List<ItemDto>>(items);
        if (currentUserId != null)
        {
            foreach (var dto in dtos)
            {
                dto.IsLikedByCurrentUser = items.First(i => i.Id == dto.Id)
                    .Likes.Any(l => l.UserId == currentUserId);
            }
        }
        return dtos;
    }
}
