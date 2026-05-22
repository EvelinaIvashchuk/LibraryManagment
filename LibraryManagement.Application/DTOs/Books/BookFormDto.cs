using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.Books;

public class BookFormDto
{
    [Required(ErrorMessage = "Вкажіть назву")]
    [MaxLength(300)]
    [Display(Name = "Назва")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть автора")]
    [MaxLength(200)]
    [Display(Name = "Автор")]
    public string Author { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть жанр")]
    [MaxLength(100)]
    [Display(Name = "Жанр")]
    public string Genre { get; set; } = string.Empty;

    [Range(1000, 2100, ErrorMessage = "Невірний рік")]
    [Display(Name = "Рік видання")]
    public int Year { get; set; } = DateTime.Now.Year;

    [Range(1, 1000, ErrorMessage = "Мінімум 1 примірник")]
    [Display(Name = "Кількість примірників")]
    public int TotalCopies { get; set; } = 1;

    [MaxLength(20)]
    [Display(Name = "ISBN")]
    public string ISBN { get; set; } = string.Empty;
}
