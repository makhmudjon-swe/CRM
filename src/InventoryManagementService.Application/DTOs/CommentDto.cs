namespace InventoryManagementService.Application.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string? AuthorName { get; set; }
    public string? AuthorAvatarUrl { get; set; }
    public int ItemId { get; set; }
}

public class CreateCommentDto
{
    public string Content { get; set; } = string.Empty;
    public int ItemId { get; set; }
}
