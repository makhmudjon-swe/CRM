using WholesaleCRM.Domain.Enums;

namespace WholesaleCRM.Domain.Entities;

public class Deal
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string? AssignedToId { get; set; }
    public DealStatus Status { get; set; } = DealStatus.Lead;
    public decimal TotalAmount { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public Customer Customer { get; set; } = null!;
    public AppUser? AssignedTo { get; set; }
    public ICollection<DealProduct> DealProducts { get; set; } = new List<DealProduct>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
