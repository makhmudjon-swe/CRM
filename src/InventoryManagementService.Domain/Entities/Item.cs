namespace InventoryManagementService.Domain.Entities;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CustomId { get; set; }

    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Custom string values
    public string? CustomString1Value { get; set; }
    public string? CustomString2Value { get; set; }
    public string? CustomString3Value { get; set; }

    // Custom text values
    public string? CustomText1Value { get; set; }
    public string? CustomText2Value { get; set; }
    public string? CustomText3Value { get; set; }

    // Custom integer values
    public int? CustomInt1Value { get; set; }
    public int? CustomInt2Value { get; set; }
    public int? CustomInt3Value { get; set; }

    // Custom link values
    public string? CustomLink1Value { get; set; }
    public string? CustomLink2Value { get; set; }
    public string? CustomLink3Value { get; set; }

    // Custom boolean values
    public bool? CustomBool1Value { get; set; }
    public bool? CustomBool2Value { get; set; }
    public bool? CustomBool3Value { get; set; }

    // Navigation
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}
