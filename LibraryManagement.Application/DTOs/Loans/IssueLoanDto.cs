using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Application.DTOs.Loans;

public class IssueLoanDto
{
    [Required(ErrorMessage = "Оберіть книгу")]
    [Display(Name = "Книга")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Оберіть читача")]
    [Display(Name = "Читач")]
    public string ReaderId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Вкажіть дату повернення")]
    [Display(Name = "Дата повернення")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(14);
}
