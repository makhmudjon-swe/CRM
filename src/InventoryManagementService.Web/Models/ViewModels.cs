using System.ComponentModel.DataAnnotations;

namespace WholesaleCRM.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Email majburiy")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parol majburiy")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ism-familiya majburiy")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email majburiy")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Parol majburiy")]
    [MinLength(6, ErrorMessage = "Parol kamida 6 ta belgi bo'lishi kerak")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Parollar mos kelmaydi")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class ErrorViewModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
