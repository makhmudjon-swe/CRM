using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Enums;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _uow;

    public CustomerService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<CustomerDto>> GetAllAsync(string? search = null)
    {
        IQueryable<Customer> query = _uow.Customers.Query()
            .Include(c => c.AssignedTo)
            .Include(c => c.Contacts)
            .Include(c => c.Deals);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c =>
                c.CompanyName.Contains(search) ||
                (c.City != null && c.City.Contains(search)) ||
                (c.Email != null && c.Email.Contains(search)));

        var customers = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetByIdAsync(int id)
    {
        var customer = await _uow.Customers.Query()
            .Include(c => c.AssignedTo)
            .Include(c => c.Contacts)
            .Include(c => c.Deals).ThenInclude(d => d.AssignedTo)
            .Include(c => c.Activities).ThenInclude(a => a.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        return customer == null ? null : MapToDtoFull(customer);
    }

    public async Task<CustomerDto> CreateAsync(CustomerRequest req)
    {
        var entity = new Customer
        {
            CompanyName = req.CompanyName,
            Industry = req.Industry,
            Email = req.Email,
            Phone = req.Phone,
            Address = req.Address,
            City = req.City,
            Country = req.Country ?? "O'zbekiston",
            Status = (CustomerStatus)req.Status,
            AssignedToId = string.IsNullOrEmpty(req.AssignedToId) ? null : req.AssignedToId,
            Notes = req.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _uow.Customers.AddAsync(entity);
        await _uow.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task<CustomerDto> UpdateAsync(int id, CustomerRequest req)
    {
        var entity = await _uow.Customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer {id} not found");

        entity.CompanyName = req.CompanyName;
        entity.Industry = req.Industry;
        entity.Email = req.Email;
        entity.Phone = req.Phone;
        entity.Address = req.Address;
        entity.City = req.City;
        entity.Country = req.Country ?? "O'zbekiston";
        entity.Status = (CustomerStatus)req.Status;
        entity.AssignedToId = string.IsNullOrEmpty(req.AssignedToId) ? null : req.AssignedToId;
        entity.Notes = req.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        _uow.Customers.Update(entity);
        await _uow.SaveChangesAsync();
        return MapToDto(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _uow.Customers.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Customer {id} not found");
        _uow.Customers.Remove(entity);
        await _uow.SaveChangesAsync();
    }

    private static CustomerDto MapToDto(Customer c) => new()
    {
        Id = c.Id,
        CompanyName = c.CompanyName,
        Industry = c.Industry,
        Email = c.Email,
        Phone = c.Phone,
        Address = c.Address,
        City = c.City,
        Country = c.Country,
        Status = EnumHelper.GetCustomerStatusName((int)c.Status),
        StatusValue = (int)c.Status,
        AssignedToId = c.AssignedToId,
        AssignedToName = c.AssignedTo?.FullName,
        Notes = c.Notes,
        ContactCount = c.Contacts?.Count ?? 0,
        DealCount = c.Deals?.Count ?? 0,
        TotalRevenue = c.Deals?.Where(d => d.Status == DealStatus.Won).Sum(d => d.TotalAmount) ?? 0,
        CreatedAt = c.CreatedAt
    };

    private static CustomerDto MapToDtoFull(Customer c)
    {
        var dto = MapToDto(c);
        dto.Contacts = c.Contacts.Select(x => new ContactDto
        {
            Id = x.Id,
            FirstName = x.FirstName,
            LastName = x.LastName,
            Email = x.Email,
            Phone = x.Phone,
            Position = x.Position,
            CustomerId = x.CustomerId,
            CustomerName = c.CompanyName,
            IsPrimary = x.IsPrimary,
            CreatedAt = x.CreatedAt
        }).ToList();

        dto.Deals = c.Deals.Select(d => new DealDto
        {
            Id = d.Id,
            Title = d.Title,
            CustomerId = d.CustomerId,
            CustomerName = c.CompanyName,
            AssignedToId = d.AssignedToId,
            AssignedToName = d.AssignedTo?.FullName,
            Status = EnumHelper.GetDealStatusName((int)d.Status),
            StatusValue = (int)d.Status,
            TotalAmount = d.TotalAmount,
            ExpectedCloseDate = d.ExpectedCloseDate,
            Notes = d.Notes,
            CreatedAt = d.CreatedAt
        }).OrderByDescending(d => d.CreatedAt).ToList();

        dto.Activities = c.Activities.Select(a => new ActivityDto
        {
            Id = a.Id,
            Type = EnumHelper.GetActivityTypeName((int)a.Type),
            TypeValue = (int)a.Type,
            TypeIcon = EnumHelper.GetActivityTypeIcon((int)a.Type),
            Subject = a.Subject,
            Notes = a.Notes,
            CustomerId = a.CustomerId,
            CustomerName = c.CompanyName,
            ActivityDate = a.ActivityDate,
            UserName = a.User?.FullName,
            CreatedAt = a.CreatedAt
        }).OrderByDescending(a => a.ActivityDate).ToList();

        return dto;
    }
}
