using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Requests;

namespace WholesaleCRM.Application.Interfaces;

public interface IContactService
{
    Task<IEnumerable<ContactDto>> GetAllAsync();
    Task<IEnumerable<ContactDto>> GetByCustomerIdAsync(int customerId);
    Task<ContactDto?> GetByIdAsync(int id);
    Task<ContactDto> CreateAsync(ContactRequest request);
    Task<ContactDto> UpdateAsync(int id, ContactRequest request);
    Task DeleteAsync(int id);
}
