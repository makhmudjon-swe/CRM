using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Application.Requests;

public class ActivityRequest
{
    [Required(ErrorMessage = "Faoliyat turi majburiy")]
    public int Type { get; set; }

    [Required(ErrorMessage = "Mavzu majburiy")]
    [MaxLength(300)]
    public string Subject { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public int? CustomerId { get; set; }

    public int? ContactId { get; set; }

    public int? DealId { get; set; }

    public string? UserId { get; set; }

    [Required(ErrorMessage = "Sana majburiy")]
    public DateTime ActivityDate { get; set; } = DateTime.Now;
}
