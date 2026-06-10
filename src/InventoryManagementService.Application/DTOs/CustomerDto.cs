namespace WholesaleCRM.Application.DTOs;

public class CustomerDto
{
    public int Id { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string Status { get; set; } = string.Empty;
    public int StatusValue { get; set; }
    public string? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public string? Notes { get; set; }
    public int ContactCount { get; set; }
    public int DealCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ContactDto> Contacts { get; set; } = new();
    public List<DealDto> Deals { get; set; } = new();
    public List<ActivityDto> Activities { get; set; } = new();
}
