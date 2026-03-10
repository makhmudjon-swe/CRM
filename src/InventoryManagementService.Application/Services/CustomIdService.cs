using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Enums;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class CustomIdService : ICustomIdService
{
    private readonly IUnitOfWork _uow;
    private static readonly Random Rng = new();

    public CustomIdService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<CustomIdFormatDto?> GetFormatAsync(int inventoryId)
    {
        var format = await _uow.CustomIdFormats.Query()
            .Include(f => f.Elements.OrderBy(e => e.SortOrder))
            .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

        if (format == null) return null;

        return MapToDto(format);
    }

    public async Task<CustomIdFormatDto> SaveFormatAsync(int inventoryId, SaveCustomIdFormatRequest request)
    {
        var format = await _uow.CustomIdFormats.Query()
            .Include(f => f.Elements)
            .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

        if (format == null)
        {
            format = new CustomIdFormat { InventoryId = inventoryId, CurrentCounter = 0 };
            await _uow.CustomIdFormats.AddAsync(format);
            await _uow.SaveChangesAsync();
        }

        // Remove old elements
        _uow.CustomIdElements.RemoveRange(format.Elements);
        await _uow.SaveChangesAsync();

        // Add new elements
        for (int i = 0; i < request.Elements.Count; i++)
        {
            var el = request.Elements[i];
            await _uow.CustomIdElements.AddAsync(new CustomIdElement
            {
                CustomIdFormatId = format.Id,
                ElementType = el.ElementType,
                Value = el.Value,
                SortOrder = i,
                PaddingLength = el.PaddingLength,
                DateFormat = el.DateFormat
            });
        }
        await _uow.SaveChangesAsync();

        return (await GetFormatAsync(inventoryId))!;
    }

    public async Task DeleteFormatAsync(int inventoryId)
    {
        var format = await _uow.CustomIdFormats.Query()
            .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);
        if (format != null)
        {
            _uow.CustomIdFormats.Remove(format);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task<string> GenerateIdAsync(int inventoryId)
    {
        var format = await _uow.CustomIdFormats.Query()
            .Include(f => f.Elements.OrderBy(e => e.SortOrder))
            .FirstOrDefaultAsync(f => f.InventoryId == inventoryId);

        if (format == null || !format.Elements.Any())
            return string.Empty;

        // Increment sequence counter
        format.CurrentCounter++;
        await _uow.SaveChangesAsync();

        return BuildId(format.Elements, format.CurrentCounter, DateTime.UtcNow);
    }

    public string PreviewId(List<SaveCustomIdElementRequest> elements)
    {
        var mapped = elements.OrderBy(e => e.SortOrder).Select(e => new CustomIdElement
        {
            ElementType = e.ElementType,
            Value = e.Value,
            PaddingLength = e.PaddingLength,
            DateFormat = e.DateFormat
        }).ToList();

        return BuildId(mapped, 42, DateTime.UtcNow);
    }

    private static string BuildId(IEnumerable<CustomIdElement> elements, int counter, DateTime now)
    {
        var parts = new List<string>();
        foreach (var el in elements)
        {
            parts.Add(el.ElementType switch
            {
                CustomIdElementType.FixedText => el.Value ?? "",
                CustomIdElementType.Sequence => FormatNumber(counter, el.PaddingLength),
                CustomIdElementType.DateTime => now.ToString(el.DateFormat ?? "yyyyMMdd"),
                CustomIdElementType.Random20Bit => FormatNumber(Rng.Next(0, 1_048_576), el.PaddingLength ?? 7),
                CustomIdElementType.Random32Bit => FormatNumber(Rng.Next(0, int.MaxValue), el.PaddingLength ?? 10),
                CustomIdElementType.Random6Digit => FormatNumber(Rng.Next(0, 1_000_000), el.PaddingLength ?? 6),
                CustomIdElementType.Random9Digit => FormatNumber(Rng.Next(0, 1_000_000_000), el.PaddingLength ?? 9),
                CustomIdElementType.Guid => System.Guid.NewGuid().ToString("N")[..(el.PaddingLength ?? 32)],
                _ => ""
            });
        }
        return string.Concat(parts);
    }

    private static string FormatNumber(int value, int? padding)
    {
        if (padding is > 0)
            return value.ToString().PadLeft(padding.Value, '0');
        return value.ToString();
    }

    private static CustomIdFormatDto MapToDto(CustomIdFormat format)
    {
        return new CustomIdFormatDto
        {
            Id = format.Id,
            InventoryId = format.InventoryId,
            CurrentCounter = format.CurrentCounter,
            Elements = format.Elements.OrderBy(e => e.SortOrder).Select(e => new CustomIdElementDto
            {
                Id = e.Id,
                ElementType = e.ElementType,
                Value = e.Value,
                SortOrder = e.SortOrder,
                PaddingLength = e.PaddingLength,
                DateFormat = e.DateFormat
            }).ToList()
        };
    }
}
