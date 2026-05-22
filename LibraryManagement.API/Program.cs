using LibraryManagement.Infrastructure;
using LibraryManagement.Infrastructure.Data;
using LibraryManagement.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (port is not null)
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    await db.Database.MigrateAsync();
    await DataSeeder.SeedAsync(scope.ServiceProvider);
}

// Healthcheck для Railway
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
