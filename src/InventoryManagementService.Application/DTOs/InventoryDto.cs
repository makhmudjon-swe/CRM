namespace InventoryManagementService.Application.DTOs;

public class InventoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string OwnerId { get; set; } = string.Empty;
    public string? OwnerName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsPublic { get; set; }
    public int ItemCount { get; set; }
    public List<string> Tags { get; set; } = new();

    // Custom field config
    public bool CustomString1Enabled { get; set; }
    public string? CustomString1Name { get; set; }
    public bool CustomString2Enabled { get; set; }
    public string? CustomString2Name { get; set; }
    public bool CustomString3Enabled { get; set; }
    public string? CustomString3Name { get; set; }

    public bool CustomText1Enabled { get; set; }
    public string? CustomText1Name { get; set; }
    public bool CustomText2Enabled { get; set; }
    public string? CustomText2Name { get; set; }
    public bool CustomText3Enabled { get; set; }
    public string? CustomText3Name { get; set; }

    public bool CustomInt1Enabled { get; set; }
    public string? CustomInt1Name { get; set; }
    public bool CustomInt2Enabled { get; set; }
    public string? CustomInt2Name { get; set; }
    public bool CustomInt3Enabled { get; set; }
    public string? CustomInt3Name { get; set; }

    public bool CustomLink1Enabled { get; set; }
    public string? CustomLink1Name { get; set; }
    public bool CustomLink2Enabled { get; set; }
    public string? CustomLink2Name { get; set; }
    public bool CustomLink3Enabled { get; set; }
    public string? CustomLink3Name { get; set; }

    public bool CustomBool1Enabled { get; set; }
    public string? CustomBool1Name { get; set; }
    public bool CustomBool2Enabled { get; set; }
    public string? CustomBool2Name { get; set; }
    public bool CustomBool3Enabled { get; set; }
    public string? CustomBool3Name { get; set; }
}

public class CreateInventoryDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int CategoryId { get; set; }
    public bool IsPublic { get; set; }
    public List<string> Tags { get; set; } = new();

    public bool CustomString1Enabled { get; set; }
    public string? CustomString1Name { get; set; }
    public bool CustomString2Enabled { get; set; }
    public string? CustomString2Name { get; set; }
    public bool CustomString3Enabled { get; set; }
    public string? CustomString3Name { get; set; }

    public bool CustomText1Enabled { get; set; }
    public string? CustomText1Name { get; set; }
    public bool CustomText2Enabled { get; set; }
    public string? CustomText2Name { get; set; }
    public bool CustomText3Enabled { get; set; }
    public string? CustomText3Name { get; set; }

    public bool CustomInt1Enabled { get; set; }
    public string? CustomInt1Name { get; set; }
    public bool CustomInt2Enabled { get; set; }
    public string? CustomInt2Name { get; set; }
    public bool CustomInt3Enabled { get; set; }
    public string? CustomInt3Name { get; set; }

    public bool CustomLink1Enabled { get; set; }
    public string? CustomLink1Name { get; set; }
    public bool CustomLink2Enabled { get; set; }
    public string? CustomLink2Name { get; set; }
    public bool CustomLink3Enabled { get; set; }
    public string? CustomLink3Name { get; set; }

    public bool CustomBool1Enabled { get; set; }
    public string? CustomBool1Name { get; set; }
    public bool CustomBool2Enabled { get; set; }
    public string? CustomBool2Name { get; set; }
    public bool CustomBool3Enabled { get; set; }
    public string? CustomBool3Name { get; set; }
}

public class UpdateInventoryDto : CreateInventoryDto
{
    public int Id { get; set; }
}
