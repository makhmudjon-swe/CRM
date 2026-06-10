using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class ContactService : IContactService
{
    private readonly IUnitOfWork _uow;

    public ContactService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ContactDto>> GetAllAsync()
    {
        var contacts = await _uow.Contacts.Query()
            .Include(c => c.Customer)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return contacts.Select(MapToDto);
    }

    public async Task<IEnumerable<ContactDto>> GetByCustomerIdAsync(int customerId)
    {
        var contacts = await _uow.Contacts.Query()
            .Include(c => c.Customer)
            .Where(c => c.CustomerId == customerId)
            .ToListAsync();
        return contacts.Select(MapToDto);
    }

    public async Task<ContactDto?> GetByIdAsync(int id)
    {
        var contact = await _uow.Contacts.Query()
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == id);
        return contact == null ? null : MapToDto(contact);
    }

    public async Task<ContactDto> CreateAsync(ContactRequest req)
    {
        var entity = new Contact
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            Phone = req.Phone,
            Position = req.Position,
            CustomerId = req.CustomerId,
            IsPrimary = req.IsPrimary,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Contacts.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var created = await _uow.Contacts.Query()
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == entity.Id);
        return MapToDto(created!);
    }

    public async Task<ContactDto> UpdateAsync(int id, ContactRequest req)
    {
        var entity = await _uow.Contacts.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contact {id} not found");

        entity.FirstName = req.FirstName;
        entity.LastName = req.LastName;
        entity.Email = req.Email;
        entity.Phone = req.Phone;
        entity.Position = req.Position;
        entity.CustomerId = req.CustomerId;
        entity.IsPrimary = req.IsPrimary;

        _uow.Contacts.Update(entity);
        await _uow.SaveChangesAsync();

        var updated = await _uow.Contacts.Query()
            .Include(c => c.Customer)
            .FirstOrDefaultAsync(c => c.Id == entity.Id);
        return MapToDto(updated!);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _uow.Contacts.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Contact {id} not found");
        _uow.Contacts.Remove(entity);
        await _uow.SaveChangesAsync();
    }

    private static ContactDto MapToDto(Contact c) => new()
    {
        Id = c.Id,
        FirstName = c.FirstName,
        LastName = c.LastName,
        Email = c.Email,
        Phone = c.Phone,
        Position = c.Position,
        CustomerId = c.CustomerId,
        CustomerName = c.Customer?.CompanyName,
        IsPrimary = c.IsPrimary,
        CreatedAt = c.CreatedAt
    };
}
