using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class BookRepository : Repository<Book>, IBookRepository
{
    public BookRepository(LibraryDbContext context) : base(context) { }

    public async Task<Book?> GetByIsbnAsync(string isbn)
        => await _dbSet.FirstOrDefaultAsync(b => b.ISBN == isbn);

    public async Task<IEnumerable<Book>> SearchAsync(string? title, string? author, string? genre)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(b => b.Title.Contains(title));

        if (!string.IsNullOrWhiteSpace(author))
            query = query.Where(b => b.Author.Contains(author));

        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(b => b.Genre == genre);

        return await query.OrderBy(b => b.Title).ToListAsync();
    }

    public async Task<IEnumerable<Book>> GetAvailableAsync()
        => await _dbSet.Where(b => b.AvailableCopies > 0).OrderBy(b => b.Title).ToListAsync();
}
