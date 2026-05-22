using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly LibraryDbContext _context;

    public IBookRepository Books { get; }
    public ILoanRepository Loans { get; }
    public IFineRepository Fines { get; }

    public UnitOfWork(LibraryDbContext context)
    {
        _context = context;
        Books = new BookRepository(context);
        Loans = new LoanRepository(context);
        Fines = new FineRepository(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
