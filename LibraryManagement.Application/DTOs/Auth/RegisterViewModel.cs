using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.Auth;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Вкажіть email")]
    [EmailAddress(ErrorMessage = "Невірний формат email")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть повне ім'я")]
    [MaxLength(200)]
    [Display(Name = "Повне ім'я")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть пароль")]
    [MinLength(8, ErrorMessage = "Мінімум 8 символів")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Підтвердіть пароль")]
    [Compare(nameof(Password), ErrorMessage = "Паролі не збігаються")]
    [DataType(DataType.Password)]
    [Display(Name = "Підтвердження пароля")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Phone]
    [MaxLength(20)]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [MaxLength(500)]
    [Display(Name = "Адреса")]
    public string? Address { get; set; }
}
