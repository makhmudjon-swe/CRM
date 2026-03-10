using InventoryManagementService.Domain.Enums;

namespace InventoryManagementService.Application.DTOs;

public class CustomIdFormatDto
{
    public int Id { get; set; }
    public int InventoryId { get; set; }
    public int CurrentCounter { get; set; }
    public List<CustomIdElementDto> Elements { get; set; } = new();
}

public class CustomIdElementDto
{
    public int Id { get; set; }
    public CustomIdElementType ElementType { get; set; }
    public string? Value { get; set; }
    public int SortOrder { get; set; }
    public int? PaddingLength { get; set; }
    public string? DateFormat { get; set; }
}

public class SaveCustomIdFormatRequest
{
    public List<SaveCustomIdElementRequest> Elements { get; set; } = new();
}

public class SaveCustomIdElementRequest
{
    public CustomIdElementType ElementType { get; set; }
    public string? Value { get; set; }
    public int SortOrder { get; set; }
    public int? PaddingLength { get; set; }
    public string? DateFormat { get; set; }
}
