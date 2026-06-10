using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Enums;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class DealService : IDealService
{
    private readonly IUnitOfWork _uow;

    public DealService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<DealDto>> GetAllAsync(int? customerId = null)
    {
        IQueryable<Deal> query = _uow.Deals.Query()
            .Include(d => d.Customer)
            .Include(d => d.AssignedTo)
            .Include(d => d.DealProducts).ThenInclude(dp => dp.Product);

        if (customerId.HasValue)
            query = query.Where(d => d.CustomerId == customerId.Value);

        var deals = await query.OrderByDescending(d => d.CreatedAt).ToListAsync();
        return deals.Select(MapToDto);
    }

    public async Task<DealDto?> GetByIdAsync(int id)
    {
        var deal = await _uow.Deals.Query()
            .Include(d => d.Customer)
            .Include(d => d.AssignedTo)
            .Include(d => d.DealProducts).ThenInclude(dp => dp.Product)
            .FirstOrDefaultAsync(d => d.Id == id);
        return deal == null ? null : MapToDto(deal);
    }

    public async Task<DealDto> CreateAsync(DealRequest req)
    {
        var entity = new Deal
        {
            Title = req.Title,
            CustomerId = req.CustomerId,
            AssignedToId = string.IsNullOrEmpty(req.AssignedToId) ? null : req.AssignedToId,
            Status = (DealStatus)req.Status,
            TotalAmount = req.TotalAmount,
            ExpectedCloseDate = req.ExpectedCloseDate,
            Notes = req.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _uow.Deals.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var created = await _uow.Deals.Query()
            .Include(d => d.Customer)
            .Include(d => d.AssignedTo)
            .FirstOrDefaultAsync(d => d.Id == entity.Id);
        return MapToDto(created!);
    }

    public async Task<DealDto> UpdateAsync(int id, DealRequest req)
    {
        var entity = await _uow.Deals.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Deal {id} not found");

        entity.Title = req.Title;
        entity.CustomerId = req.CustomerId;
        entity.AssignedToId = string.IsNullOrEmpty(req.AssignedToId) ? null : req.AssignedToId;
        entity.Status = (DealStatus)req.Status;
        entity.TotalAmount = req.TotalAmount;
        entity.ExpectedCloseDate = req.ExpectedCloseDate;
        entity.Notes = req.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        _uow.Deals.Update(entity);
        await _uow.SaveChangesAsync();

        var updated = await _uow.Deals.Query()
            .Include(d => d.Customer)
            .Include(d => d.AssignedTo)
            .FirstOrDefaultAsync(d => d.Id == entity.Id);
        return MapToDto(updated!);
    }

    public async Task UpdateStatusAsync(int id, int status)
    {
        var entity = await _uow.Deals.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Deal {id} not found");
        entity.Status = (DealStatus)status;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Deals.Update(entity);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _uow.Deals.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Deal {id} not found");
        _uow.Deals.Remove(entity);
        await _uow.SaveChangesAsync();
    }

    private static DealDto MapToDto(Deal d) => new()
    {
        Id = d.Id,
        Title = d.Title,
        CustomerId = d.CustomerId,
        CustomerName = d.Customer?.CompanyName,
        AssignedToId = d.AssignedToId,
        AssignedToName = d.AssignedTo?.FullName,
        Status = EnumHelper.GetDealStatusName((int)d.Status),
        StatusValue = (int)d.Status,
        TotalAmount = d.TotalAmount,
        ExpectedCloseDate = d.ExpectedCloseDate,
        Notes = d.Notes,
        CreatedAt = d.CreatedAt,
        Products = d.DealProducts?.Select(dp => new DealProductDto
        {
            Id = dp.Id,
            ProductId = dp.ProductId,
            ProductName = dp.Product?.Name,
            Quantity = dp.Quantity,
            UnitPrice = dp.UnitPrice,
            Discount = dp.Discount,
            TotalPrice = dp.TotalPrice
        }).ToList() ?? new()
    };
}
