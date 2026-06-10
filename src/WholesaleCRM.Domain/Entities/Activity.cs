using WholesaleCRM.Domain.Enums;

namespace WholesaleCRM.Domain.Entities;

public class Activity
{
    public int Id { get; set; }
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? CustomerId { get; set; }
    public int? ContactId { get; set; }
    public int? DealId { get; set; }
    public string? UserId { get; set; }
    public DateTime ActivityDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Customer? Customer { get; set; }
    public Contact? Contact { get; set; }
    public Deal? Deal { get; set; }
    public AppUser? User { get; set; }
}
