using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class CommentService : ICommentService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CommentService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CommentDto>> GetByItemAsync(int itemId)
    {
        var comments = await _uow.Comments.Query()
            .Include(c => c.Author)
            .Where(c => c.ItemId == itemId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CommentDto>>(comments);
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto, string authorId)
    {
        var comment = new Comment
        {
            Content = dto.Content,
            ItemId = dto.ItemId,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Comments.AddAsync(comment);
        await _uow.SaveChangesAsync();

        var created = await _uow.Comments.Query()
            .Include(c => c.Author)
            .FirstAsync(c => c.Id == comment.Id);
        return _mapper.Map<CommentDto>(created);
    }

    public async Task DeleteAsync(int id)
    {
        var comment = await _uow.Comments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Comment {id} not found");
        _uow.Comments.Remove(comment);
        await _uow.SaveChangesAsync();
    }
}
