using InventoryManagementService.Application.DTOs;

namespace InventoryManagementService.Application.Interfaces;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByItemAsync(int itemId);
    Task<CommentDto> CreateAsync(CreateCommentDto dto, string authorId);
    Task DeleteAsync(int id);
}
