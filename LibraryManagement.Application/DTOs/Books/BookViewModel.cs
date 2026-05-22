namespace LibraryManagement.Application.DTOs.Books;

public class BookViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Year { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public bool IsAvailable => AvailableCopies > 0;
}
