using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class TagService : ITagService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public TagService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TagDto>> GetAllAsync()
    {
        var tags = await _uow.Tags.Query()
            .Include(t => t.InventoryTags)
            .OrderBy(t => t.Name)
            .ToListAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<IEnumerable<TagDto>> GetPopularAsync(int count)
    {
        var tags = await _uow.Tags.Query()
            .Include(t => t.InventoryTags)
            .OrderByDescending(t => t.InventoryTags.Count)
            .Take(count)
            .ToListAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }

    public async Task<IEnumerable<TagDto>> SearchAsync(string query)
    {
        var lowerQuery = query.ToLower();
        var tags = await _uow.Tags.Query()
            .Include(t => t.InventoryTags)
            .Where(t => t.Name.ToLower().Contains(lowerQuery))
            .Take(10)
            .ToListAsync();
        return _mapper.Map<IEnumerable<TagDto>>(tags);
    }
}
