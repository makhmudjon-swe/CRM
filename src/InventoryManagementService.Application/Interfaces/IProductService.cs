using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(int? categoryId = null);
    Task<ProductDto?> GetByIdAsync(int id);
    Task<IEnumerable<ProductCategoryDto>> GetCategoriesAsync();
    Task<ProductDto> CreateAsync(ProductRequest request);
    Task<ProductDto> UpdateAsync(int id, ProductRequest request);
    Task DeleteAsync(int id);
}
