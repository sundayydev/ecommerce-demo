using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shop.Domain.Constants;
using Shop.Domain.Entities;

namespace Shop.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseSeeder");

        try
        {
            if (context.Database.IsRelational())
            {
                await context.Database.MigrateAsync();
            }

            var adminEmail = "sundayy.dev@shop.com"; 
            
            if (!await context.Users.AnyAsync(u => u.Email == adminEmail))
            {
                var adminUser = new User
                {
                    Email = adminEmail,
                    FullName = "Admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), 
                    Role = Roles.Admin
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
                logger.LogInformation("Đã tự động tạo tài khoản Admin mặc định thành công!");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Có lỗi xảy ra trong quá trình Seed dữ liệu.");
            throw;
        }
    }
}