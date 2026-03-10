using InventoryManagementService.Domain.Enums;

namespace InventoryManagementService.Domain.Entities;

public class CustomIdElement
{
    public int Id { get; set; }

    public int CustomIdFormatId { get; set; }
    public CustomIdFormat CustomIdFormat { get; set; } = null!;

    public CustomIdElementType ElementType { get; set; }
    public string? Value { get; set; }
    public int SortOrder { get; set; }
    public int? PaddingLength { get; set; }
    public string? DateFormat { get; set; }
}
