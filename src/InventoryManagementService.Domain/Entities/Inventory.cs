namespace InventoryManagementService.Domain.Entities;

public class Inventory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public string OwnerId { get; set; } = string.Empty;
    public ApplicationUser Owner { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsPublic { get; set; }

    // Custom string fields (3)
    public bool CustomString1Enabled { get; set; }
    public string? CustomString1Name { get; set; }
    public bool CustomString2Enabled { get; set; }
    public string? CustomString2Name { get; set; }
    public bool CustomString3Enabled { get; set; }
    public string? CustomString3Name { get; set; }

    // Custom text fields (3) - multiline
    public bool CustomText1Enabled { get; set; }
    public string? CustomText1Name { get; set; }
    public bool CustomText2Enabled { get; set; }
    public string? CustomText2Name { get; set; }
    public bool CustomText3Enabled { get; set; }
    public string? CustomText3Name { get; set; }

    // Custom integer fields (3)
    public bool CustomInt1Enabled { get; set; }
    public string? CustomInt1Name { get; set; }
    public bool CustomInt2Enabled { get; set; }
    public string? CustomInt2Name { get; set; }
    public bool CustomInt3Enabled { get; set; }
    public string? CustomInt3Name { get; set; }

    // Custom link fields (3)
    public bool CustomLink1Enabled { get; set; }
    public string? CustomLink1Name { get; set; }
    public bool CustomLink2Enabled { get; set; }
    public string? CustomLink2Name { get; set; }
    public bool CustomLink3Enabled { get; set; }
    public string? CustomLink3Name { get; set; }

    // Custom boolean fields (3)
    public bool CustomBool1Enabled { get; set; }
    public string? CustomBool1Name { get; set; }
    public bool CustomBool2Enabled { get; set; }
    public string? CustomBool2Name { get; set; }
    public bool CustomBool3Enabled { get; set; }
    public string? CustomBool3Name { get; set; }

    // Navigation
    public ICollection<Item> Items { get; set; } = new List<Item>();
    public ICollection<InventoryTag> InventoryTags { get; set; } = new List<InventoryTag>();
    public ICollection<CustomIdFormat> CustomIdFormats { get; set; } = new List<CustomIdFormat>();
    public ICollection<InventoryAccess> AccessList { get; set; } = new List<InventoryAccess>();
}
