using AutoMapper;
using InventoryManagementService.Application.DTOs;
using InventoryManagementService.Application.Interfaces;
using InventoryManagementService.Domain.Entities;
using InventoryManagementService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementService.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _uow.Categories.Query()
            .OrderBy(c => c.Name)
            .ToListAsync();
        return _mapper.Map<IEnumerable<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> CreateAsync(string name, string? description = null)
    {
        var category = new Category { Name = name, Description = description };
        await _uow.Categories.AddAsync(category);
        await _uow.SaveChangesAsync();
        return _mapper.Map<CategoryDto>(category);
    }
}
