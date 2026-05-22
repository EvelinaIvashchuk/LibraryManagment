using LibraryManagement.Application.DTOs.Readers;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Entities.Enums;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.API.Controllers;

[Authorize(Roles = "Admin,Librarian")]
public class ReadersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _uow;

    public ReadersController(UserManager<ApplicationUser> userManager, IUnitOfWork uow)
    {
        _userManager = userManager;
        _uow = uow;
    }

    // GET /Readers
    public async Task<IActionResult> Index(string? search)
    {
        var users = await _userManager.Users.ToListAsync();

        // Фільтруємо тільки читачів (не адмінів і не бібліотекарів)
        var readers = new List<ApplicationUser>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Reader"))
                readers.Add(user);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            readers = readers
                .Where(r => r.FullName.Contains(search, StringComparison.OrdinalIgnoreCase)
                         || (r.Email ?? "").Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var vms = new List<ReaderViewModel>();
        foreach (var r in readers)
        {
            var activeLoans = (await _uow.Loans.GetByReaderIdAsync(r.Id))
                .Count(l => l.Status == LoanStatus.Active);
            var unpaidFines = await _uow.Fines.GetTotalUnpaidByReaderAsync(r.Id);

            vms.Add(new ReaderViewModel
            {
                Id = r.Id,
                FullName = r.FullName,
                Email = r.Email ?? "",
                Phone = r.Phone,
                Address = r.Address,
                RegisteredAt = r.RegisteredAt,
                IsBlocked = r.IsBlocked,
                ActiveLoans = activeLoans,
                UnpaidFines = unpaidFines
            });
        }

        ViewBag.Search = search;
        return View(vms.OrderBy(r => r.FullName));
    }

    // GET /Readers/Details/id
    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var loans = await _uow.Loans.GetByReaderIdAsync(id);
        var unpaidFines = await _uow.Fines.GetTotalUnpaidByReaderAsync(id);

        var vm = new ReaderViewModel
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? "",
            Phone = user.Phone,
            Address = user.Address,
            RegisteredAt = user.RegisteredAt,
            IsBlocked = user.IsBlocked,
            ActiveLoans = loans.Count(l => l.Status == LoanStatus.Active),
            UnpaidFines = unpaidFines
        };

        ViewBag.Loans = loans.OrderByDescending(l => l.IssuedAt).Take(20).ToList();
        return View(vm);
    }

    // POST /Readers/Block/id
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Block(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = $"Читача «{user.FullName}» заблоковано";
        return RedirectToAction(nameof(Details), new { id });
    }

    // POST /Readers/Unblock/id
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Unblock(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        user.IsBlocked = false;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = $"Читача «{user.FullName}» розблоковано";
        return RedirectToAction(nameof(Details), new { id });
    }
}
