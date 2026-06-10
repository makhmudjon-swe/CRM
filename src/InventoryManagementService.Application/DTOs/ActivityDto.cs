namespace WholesaleCRM.Application.DTOs;

public class ActivityDto
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public int TypeValue { get; set; }
    public string TypeIcon { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int? ContactId { get; set; }
    public string? ContactName { get; set; }
    public int? DealId { get; set; }
    public string? DealTitle { get; set; }
    public string? UserName { get; set; }
    public DateTime ActivityDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
