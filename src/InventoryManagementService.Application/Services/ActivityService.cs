using Microsoft.EntityFrameworkCore;
using WholesaleCRM.Application.DTOs;
using WholesaleCRM.Application.Interfaces;
using WholesaleCRM.Application.Requests;
using WholesaleCRM.Domain.Entities;
using WholesaleCRM.Domain.Enums;
using WholesaleCRM.Domain.Interfaces;

namespace WholesaleCRM.Application.Services;

public class ActivityService : IActivityService
{
    private readonly IUnitOfWork _uow;

    public ActivityService(IUnitOfWork uow) => _uow = uow;

    public async Task<IEnumerable<ActivityDto>> GetAllAsync(int? customerId = null, int? dealId = null)
    {
        IQueryable<Activity> query = _uow.Activities.Query()
            .Include(a => a.Customer)
            .Include(a => a.Contact)
            .Include(a => a.Deal)
            .Include(a => a.User);

        if (customerId.HasValue) query = query.Where(a => a.CustomerId == customerId.Value);
        if (dealId.HasValue) query = query.Where(a => a.DealId == dealId.Value);

        var activities = await query.OrderByDescending(a => a.ActivityDate).ToListAsync();
        return activities.Select(MapToDto);
    }

    public async Task<IEnumerable<ActivityDto>> GetRecentAsync(int count = 10)
    {
        var activities = await _uow.Activities.Query()
            .Include(a => a.Customer)
            .Include(a => a.User)
            .Include(a => a.Deal)
            .OrderByDescending(a => a.ActivityDate)
            .Take(count)
            .ToListAsync();
        return activities.Select(MapToDto);
    }

    public async Task<ActivityDto> CreateAsync(ActivityRequest req)
    {
        var entity = new Activity
        {
            Type = (ActivityType)req.Type,
            Subject = req.Subject,
            Notes = req.Notes,
            CustomerId = req.CustomerId,
            ContactId = req.ContactId,
            DealId = req.DealId,
            UserId = string.IsNullOrEmpty(req.UserId) ? null : req.UserId,
            ActivityDate = req.ActivityDate,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Activities.AddAsync(entity);
        await _uow.SaveChangesAsync();

        var created = await _uow.Activities.Query()
            .Include(a => a.Customer)
            .Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == entity.Id);
        return MapToDto(created!);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _uow.Activities.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Activity {id} not found");
        _uow.Activities.Remove(entity);
        await _uow.SaveChangesAsync();
    }

    private static ActivityDto MapToDto(Activity a) => new()
    {
        Id = a.Id,
        Type = EnumHelper.GetActivityTypeName((int)a.Type),
        TypeValue = (int)a.Type,
        TypeIcon = EnumHelper.GetActivityTypeIcon((int)a.Type),
        Subject = a.Subject,
        Notes = a.Notes,
        CustomerId = a.CustomerId,
        CustomerName = a.Customer?.CompanyName,
        ContactId = a.ContactId,
        ContactName = a.Contact != null ? $"{a.Contact.FirstName} {a.Contact.LastName}" : null,
        DealId = a.DealId,
        DealTitle = a.Deal?.Title,
        UserName = a.User?.FullName,
        ActivityDate = a.ActivityDate,
        CreatedAt = a.CreatedAt
    };
}
