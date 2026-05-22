using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var context = serviceProvider.GetRequiredService<Data.LibraryDbContext>();
        var logger = serviceProvider.GetRequiredService<ILogger<Data.LibraryDbContext>>();

        await SeedRolesAsync(roleManager, logger);
        await SeedAdminAsync(userManager, logger);
        await SeedReadersAsync(userManager, logger);
        await SeedBooksAsync(context, logger);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger)
    {
        string[] roles = ["Admin", "Librarian", "Reader"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                logger.LogInformation("Created role: {Role}", role);
            }
        }
    }

    private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        const string adminEmail = "admin@library.com";

        if (await userManager.FindByEmailAsync(adminEmail) is not null)
            return;

        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
            FullName = "System Administrator",
            Phone = "+380000000000",
            Address = "Library HQ",
            RegisteredAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
            logger.LogInformation("Admin user created: {Email}", adminEmail);
        }
        else
        {
            logger.LogError("Failed to create admin: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    private static async Task SeedReadersAsync(UserManager<ApplicationUser> userManager, ILogger logger)
    {
        var readers = new[]
        {
            new
            {
                Email = "ivan.petrenko@example.com",
                Password = "Reader123!",
                FullName = "Іван Петренко",
                Phone = "+380671234567",
                Address = "вул. Хрещатик 1, Київ"
            },
            new
            {
                Email = "olena.kovalenko@example.com",
                Password = "Reader123!",
                FullName = "Олена Коваленко",
                Phone = "+380507654321",
                Address = "вул. Шевченка 15, Львів"
            }
        };

        foreach (var data in readers)
        {
            if (await userManager.FindByEmailAsync(data.Email) is not null)
                continue;

            var user = new ApplicationUser
            {
                UserName = data.Email,
                Email = data.Email,
                EmailConfirmed = true,
                FullName = data.FullName,
                Phone = data.Phone,
                Address = data.Address,
                RegisteredAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(user, data.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Reader");
                logger.LogInformation("Reader created: {Email}", data.Email);
            }
        }
    }

    private static async Task SeedBooksAsync(Data.LibraryDbContext context, ILogger logger)
    {
        if (await context.Books.AnyAsync())
            return;

        var books = new List<Core.Entities.Book>
        {
            new() { Title = "Кобзар", Author = "Тарас Шевченко", Genre = "Поезія", Year = 1840, TotalCopies = 5, AvailableCopies = 5, ISBN = "978-966-01-0001-1" },
            new() { Title = "Тіні забутих предків", Author = "Михайло Коцюбинський", Genre = "Повість", Year = 1913, TotalCopies = 4, AvailableCopies = 4, ISBN = "978-966-01-0002-2" },
            new() { Title = "Місто", Author = "Валер'ян Підмогильний", Genre = "Роман", Year = 1928, TotalCopies = 3, AvailableCopies = 3, ISBN = "978-966-01-0003-3" },
            new() { Title = "Майстер і Маргарита", Author = "Михайло Булгаков", Genre = "Роман", Year = 1967, TotalCopies = 6, AvailableCopies = 6, ISBN = "978-5-17-0004-4" },
            new() { Title = "1984", Author = "Джордж Орвелл", Genre = "Антиутопія", Year = 1949, TotalCopies = 5, AvailableCopies = 5, ISBN = "978-0-452-28423-4" },
            new() { Title = "Гаррі Поттер і Філософський камінь", Author = "Дж. К. Роулінг", Genre = "Фентезі", Year = 1997, TotalCopies = 8, AvailableCopies = 8, ISBN = "978-0-7475-3269-9" },
            new() { Title = "Злочин і кара", Author = "Федір Достоєвський", Genre = "Роман", Year = 1866, TotalCopies = 4, AvailableCopies = 4, ISBN = "978-966-01-0007-7" },
            new() { Title = "Маленький принц", Author = "Антуан де Сент-Екзюпері", Genre = "Казка", Year = 1943, TotalCopies = 7, AvailableCopies = 7, ISBN = "978-966-01-0008-8" },
            new() { Title = "Вбити пересмішника", Author = "Гарпер Лі", Genre = "Роман", Year = 1960, TotalCopies = 4, AvailableCopies = 4, ISBN = "978-0-06-112008-4" },
            new() { Title = "Сто років самотності", Author = "Габріель Гарсіа Маркес", Genre = "Магічний реалізм", Year = 1967, TotalCopies = 3, AvailableCopies = 3, ISBN = "978-0-06-088328-7" },
            new() { Title = "Чистий код", Author = "Роберт Мартін", Genre = "Технічна", Year = 2008, TotalCopies = 5, AvailableCopies = 5, ISBN = "978-0-13-235088-4" },
            new() { Title = "Архітектура програмного забезпечення", Author = "Мартін Фаулер", Genre = "Технічна", Year = 2002, TotalCopies = 3, AvailableCopies = 3, ISBN = "978-0-32-112521-7" },
            new() { Title = "Атлант розправив плечі", Author = "Айн Ренд", Genre = "Роман", Year = 1957, TotalCopies = 4, AvailableCopies = 4, ISBN = "978-0-45-201036-3" },
            new() { Title = "Дюна", Author = "Френк Герберт", Genre = "Наукова фантастика", Year = 1965, TotalCopies = 5, AvailableCopies = 5, ISBN = "978-0-44-101015-5" },
            new() { Title = "Гра престолів", Author = "Джордж Р. Р. Мартін", Genre = "Фентезі", Year = 1996, TotalCopies = 6, AvailableCopies = 6, ISBN = "978-0-55-357340-3" }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} books", books.Count);
    }
}
