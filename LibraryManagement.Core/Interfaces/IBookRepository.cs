using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<Book?> GetByIsbnAsync(string isbn);
    Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? genre);
    Task<IEnumerable<Book>> GetAvailableAsync();
}
