using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Application.Interfaces;

public interface IActivityService
{
    Task<IEnumerable<ActivityDto>> GetAllAsync(int? customerId = null, int? dealId = null);
    Task<IEnumerable<ActivityDto>> GetRecentAsync(int count = 10);
    Task<ActivityDto> CreateAsync(ActivityRequest request);
    Task DeleteAsync(int id);
}
