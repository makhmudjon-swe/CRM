using Microsoft.AspNetCore.Identity;

namespace WholesaleCRM.Domain.Entities;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Customer> AssignedCustomers { get; set; } = new List<Customer>();
    public ICollection<Deal> AssignedDeals { get; set; } = new List<Deal>();
    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
