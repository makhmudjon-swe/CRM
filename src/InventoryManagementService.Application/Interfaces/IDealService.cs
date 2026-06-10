using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Application.Interfaces;

public interface IDealService
{
    Task<IEnumerable<DealDto>> GetAllAsync(int? customerId = null);
    Task<DealDto?> GetByIdAsync(int id);
    Task<DealDto> CreateAsync(DealRequest request);
    Task<DealDto> UpdateAsync(int id, DealRequest request);
    Task UpdateStatusAsync(int id, int status);
    Task DeleteAsync(int id);
}
