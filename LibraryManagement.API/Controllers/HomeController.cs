using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.DTOs.Loans;
using LibraryManagement.Core.Entities.Enums;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LibraryManagement.Core.Entities;
using Microsoft.EntityFrameworkCore;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.API.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly LibraryDbContext _context;

    public HomeController(IUnitOfWork uow, LibraryDbContext context)
    {
        _uow = uow;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var allBooks = await _uow.Books.GetAllAsync();
        var activeLoans = (await _uow.Loans.GetByStatusAsync(LoanStatus.Active)).ToList();
        var overdueLoans = await _uow.Loans.GetOverdueAsync();
        var readers = await _context.Users.CountAsync();
        var unpaidFines = await _uow.Fines.GetUnpaidAsync();

        var recentLoans = activeLoans
            .OrderByDescending(l => l.IssuedAt)
            .Take(10)
            .Select(l => new LoanViewModel
            {
                Id = l.Id,
                BookId = l.BookId,
                BookTitle = l.Book?.Title ?? "",
                BookAuthor = l.Book?.Author ?? "",
                ReaderId = l.ReaderId,
                ReaderName = l.Reader?.FullName ?? "",
                IssuedAt = l.IssuedAt,
                DueDate = l.DueDate,
                Status = l.Status
            });

        var vm = new DashboardViewModel
        {
            TotalBooks = allBooks.Count(),
            TotalReaders = readers,
            ActiveLoans = activeLoans.Count,
            OverdueLoans = overdueLoans.Count(),
            UnpaidFines = unpaidFines.Sum(f => f.Amount),
            RecentLoans = recentLoans
        };

        return View(vm);
    }
}
