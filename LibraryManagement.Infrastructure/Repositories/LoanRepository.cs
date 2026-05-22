using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Enums;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Infrastructure.Repositories;

public class LoanRepository : Repository<Loan>, ILoanRepository
{
    public LoanRepository(LibraryDbContext context) : base(context) { }

    public async Task<IEnumerable<Loan>> GetByReaderIdAsync(string readerId)
        => await _dbSet
            .Include(l => l.Book)
            .Where(l => l.ReaderId == readerId)
            .OrderByDescending(l => l.IssuedAt)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetByStatusAsync(LoanStatus status)
        => await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Reader)
            .Where(l => l.Status == status)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetOverdueAsync()
        => await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Reader)
            .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.UtcNow)
            .ToListAsync();

    public async Task<Loan?> GetWithDetailsAsync(int id)
        => await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Reader)
            .Include(l => l.Fine)
            .FirstOrDefaultAsync(l => l.Id == id);
}
