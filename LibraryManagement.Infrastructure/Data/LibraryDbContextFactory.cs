using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LibraryManagement.Infrastructure.Data;

// Використовується тільки EF CLI (ef migrations add/update-database) без реального з'єднання
public class LibraryDbContextFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseMySql(
                "Server=localhost;Port=3306;Database=library_management;User=root;Password=12345678;AllowPublicKeyRetrieval=True;SslMode=Preferred;",
                new MySqlServerVersion(new Version(8, 0, 36)),
                b => b.MigrationsAssembly(typeof(LibraryDbContext).Assembly.FullName))
            .Options;

        return new LibraryDbContext(options);
    }
}
