using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllAsync(string? search = null);
    Task<CustomerDto?> GetByIdAsync(int id);
    Task<CustomerDto> CreateAsync(CustomerRequest request);
    Task<CustomerDto> UpdateAsync(int id, CustomerRequest request);
    Task DeleteAsync(int id);
}
