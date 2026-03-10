namespace InventoryManagementService.Application.DTOs;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? CustomId { get; set; }
    public int InventoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }

    public string? CustomString1Value { get; set; }
    public string? CustomString2Value { get; set; }
    public string? CustomString3Value { get; set; }
    public string? CustomText1Value { get; set; }
    public string? CustomText2Value { get; set; }
    public string? CustomText3Value { get; set; }
    public int? CustomInt1Value { get; set; }
    public int? CustomInt2Value { get; set; }
    public int? CustomInt3Value { get; set; }
    public string? CustomLink1Value { get; set; }
    public string? CustomLink2Value { get; set; }
    public string? CustomLink3Value { get; set; }
    public bool? CustomBool1Value { get; set; }
    public bool? CustomBool2Value { get; set; }
    public bool? CustomBool3Value { get; set; }
}

public class CreateItemDto
{
    public string Name { get; set; } = string.Empty;
    public int InventoryId { get; set; }

    public string? CustomString1Value { get; set; }
    public string? CustomString2Value { get; set; }
    public string? CustomString3Value { get; set; }
    public string? CustomText1Value { get; set; }
    public string? CustomText2Value { get; set; }
    public string? CustomText3Value { get; set; }
    public int? CustomInt1Value { get; set; }
    public int? CustomInt2Value { get; set; }
    public int? CustomInt3Value { get; set; }
    public string? CustomLink1Value { get; set; }
    public string? CustomLink2Value { get; set; }
    public string? CustomLink3Value { get; set; }
    public bool? CustomBool1Value { get; set; }
    public bool? CustomBool2Value { get; set; }
    public bool? CustomBool3Value { get; set; }
}

public class UpdateItemDto : CreateItemDto
{
    public int Id { get; set; }
    public string? CustomId { get; set; }
}
