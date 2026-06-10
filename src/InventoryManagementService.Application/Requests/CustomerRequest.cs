using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Application.Requests;

public class CustomerRequest
{
    [Required(ErrorMessage = "Kompaniya nomi majburiy")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Industry { get; set; }

    [EmailAddress(ErrorMessage = "Noto'g'ri email format")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Noto'g'ri telefon format")]
    public string? Phone { get; set; }

    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    [MaxLength(100)]
    public string? Country { get; set; }

    public int Status { get; set; }

    public string? AssignedToId { get; set; }

    public string? Notes { get; set; }
}
