namespace InventoryManagementService.Domain.Entities;

public class CustomIdFormat
{
    public int Id { get; set; }

    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public int CurrentCounter { get; set; }

    public ICollection<CustomIdElement> Elements { get; set; } = new List<CustomIdElement>();
}
