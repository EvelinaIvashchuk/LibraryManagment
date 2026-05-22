using LibraryManagement.Application.DTOs.Books;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[Authorize]
public class BooksController : Controller
{
    private readonly IUnitOfWork _uow;

    public BooksController(IUnitOfWork uow) => _uow = uow;

    // GET /Books
    public async Task<IActionResult> Index(string? search, string? genre)
    {
        var books = await _uow.Books.SearchAsync(search, null, genre);
        var vms = books.Select(MapToViewModel);

        ViewBag.Search = search;
        ViewBag.Genre = genre;
        return View(vms);
    }

    // GET /Books/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var book = await _uow.Books.GetByIdAsync(id);
        if (book is null) return NotFound();
        return View(MapToViewModel(book));
    }

    // GET /Books/Create
    [Authorize(Roles = "Admin,Librarian")]
    public IActionResult Create() => View(new BookFormDto());

    // POST /Books/Create
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create(BookFormDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        if (!string.IsNullOrWhiteSpace(dto.ISBN))
        {
            var existing = await _uow.Books.GetByIsbnAsync(dto.ISBN);
            if (existing is not null)
            {
                ModelState.AddModelError(nameof(dto.ISBN), "Книга з таким ISBN вже існує");
                return View(dto);
            }
        }

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Genre = dto.Genre,
            Year = dto.Year,
            TotalCopies = dto.TotalCopies,
            AvailableCopies = dto.TotalCopies,
            ISBN = dto.ISBN
        };

        await _uow.Books.AddAsync(book);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Книгу «{book.Title}» успішно додано";
        return RedirectToAction(nameof(Index));
    }

    // GET /Books/Edit/5
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _uow.Books.GetByIdAsync(id);
        if (book is null) return NotFound();

        return View(new BookFormDto
        {
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            Year = book.Year,
            TotalCopies = book.TotalCopies,
            ISBN = book.ISBN
        });
    }

    // POST /Books/Edit/5
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Edit(int id, BookFormDto dto)
    {
        if (!ModelState.IsValid) return View(dto);

        var book = await _uow.Books.GetByIdAsync(id);
        if (book is null) return NotFound();

        // Коригуємо доступні примірники відповідно до зміни загальної кількості
        var diff = dto.TotalCopies - book.TotalCopies;
        book.Title = dto.Title;
        book.Author = dto.Author;
        book.Genre = dto.Genre;
        book.Year = dto.Year;
        book.ISBN = dto.ISBN;
        book.TotalCopies = dto.TotalCopies;
        book.AvailableCopies = Math.Max(0, book.AvailableCopies + diff);

        _uow.Books.Update(book);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Книгу «{book.Title}» оновлено";
        return RedirectToAction(nameof(Index));
    }

    // GET /Books/Delete/5
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _uow.Books.GetByIdAsync(id);
        if (book is null) return NotFound();
        return View(MapToViewModel(book));
    }

    // POST /Books/Delete/5
    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _uow.Books.GetByIdAsync(id);
        if (book is null) return NotFound();

        _uow.Books.Remove(book);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"Книгу «{book.Title}» видалено";
        return RedirectToAction(nameof(Index));
    }

    private static BookViewModel MapToViewModel(Book b) => new()
    {
        Id = b.Id,
        Title = b.Title,
        Author = b.Author,
        Genre = b.Genre,
        Year = b.Year,
        TotalCopies = b.TotalCopies,
        AvailableCopies = b.AvailableCopies,
        ISBN = b.ISBN
    };
}
