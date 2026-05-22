using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class FineRepository : Repository<Fine>, IFineRepository
{
    public FineRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<Fine>> GetByReaderIdAsync(string readerId)
        => await _dbSet
            .Include(f => f.Loan).ThenInclude(l => l.Book)
            .Where(f => f.ReaderId == readerId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<Fine>> GetUnpaidAsync()
        => await _dbSet
            .Include(f => f.Reader)
            .Include(f => f.Loan).ThenInclude(l => l.Book)
            .Where(f => !f.IsPaid)
            .ToListAsync();

    public async Task<decimal> GetTotalUnpaidByReaderAsync(string readerId)
        => await _dbSet
            .Where(f => f.ReaderId == readerId && !f.IsPaid)
            .SumAsync(f => f.Amount);
}
