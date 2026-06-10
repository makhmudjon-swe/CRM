using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _uow;

    public ProductService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ProductDto>> GetAllAsync(int? categoryId = null)
    {
        IQueryable<Product> query = _uow.Products.Query().Include(p => p.Category);
        if (categoryId.HasValue)
            query = query.Where(p => p.ProductCategoryId == categoryId.Value);

        var products = await query.OrderBy(p => p.Name).ToListAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _uow.Products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        return product == null ? null : MapToDto(product);
    }

    public async Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsync()
    {
        var cats = await _uow.ProductCategories.Query()
            .Include(c => c.Products)
            .OrderBy(c => c.Name)
            .ToListAsync();
        return cats.Select(c => new ProductCategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ProductCount = c.Products.Count
        });
    }

    public async Task<ProductDto> CreateAsync(ProductRequest req)
    {
        var entity = new Product
        {
            Name = req.Name,
            SKU = req.SKU,
            Description = req.Description,
            ProductCategoryId = req.ProductCategoryId,
            UnitPrice = req.UnitPrice,
            StockQuantity = req.StockQuantity,
            IsActive = req.IsActive,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Products.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var created = await _uow.Products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == entity.Id);
        return MapToDto(created!);
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductRequest req)
    {
        var entity = await _uow.Products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found");

        entity.Name = req.Name;
        entity.SKU = req.SKU;
        entity.Description = req.Description;
        entity.ProductCategoryId = req.ProductCategoryId;
        entity.UnitPrice = req.UnitPrice;
        entity.StockQuantity = req.StockQuantity;
        entity.IsActive = req.IsActive;

        _uow.Products.Update(entity);
        await _uow.SaveChangesAsync();

        var updated = await _uow.Products.Query()
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == entity.Id);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _uow.Products.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found");
        _uow.Products.Remove(entity);
        await _uow.SaveChangesAsync();
    }

    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        SKU = p.SKU,
        Description = p.Description,
        ProductCategoryId = p.ProductCategoryId,
        CategoryName = p.Category?.Name,
        UnitPrice = p.UnitPrice,
        StockQuantity = p.StockQuantity,
        IsActive = p.IsActive,
        CreatedAt = p.CreatedAt
    };
}
