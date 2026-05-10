using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Context;
using Wardrobe.Repositories.Helpers;

namespace Wardrobe.Repositories.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseSeeder");

        logger.LogInformation("Database seeding started");

        try
        {
            await context.Database.MigrateAsync();

            await SeedRolesAsync(context, logger);
            await SeedAdminAsync(context, logger);
            await SeedCategoriesAsync(context, logger);
            await SeedBrandsAsync(context, logger);

            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Database seeding failed");
            throw;
        }
    }

    private static async Task SeedRolesAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Roles.AnyAsync())
        {
            logger.LogInformation("Roles already exist, skipping");
            return;
        }

        logger.LogInformation("Seeding roles");

        var roles = new List<Role>
        {
            new Role { Name = "Admin" },
            new Role { Name = "User" }
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();

        logger.LogInformation("Roles seeded: Admin, User");
    }

    private static async Task SeedAdminAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Users.AnyAsync())
        {
            logger.LogInformation("Users already exist, skipping");
            return;
        }

        logger.LogInformation("Seeding admin user");

        var adminRole = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == "Admin");

        if (adminRole == null)
        {
            logger.LogError("Admin role not found! Make sure roles are seeded first.");
            return;
        }

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@wardrobe.local",
            PasswordHash = PasswordHasher.Hash("Admin123!"),
            RoleId = adminRole.Id
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        logger.LogInformation("Admin user seeded: admin@wardrobe.local / Admin123!");
    }

    private static async Task SeedCategoriesAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync())
        {
            logger.LogInformation("Categories already exist, skipping");
            return;
        }

        logger.LogInformation("Seeding categories");

        var categories = new List<Category>
        {
            new Category { Name = "Shoes" },
            new Category { Name = "T-Shirts" },
            new Category { Name = "Jeans" },
            new Category { Name = "Jackets" },
            new Category { Name = "Accessories" }
        };

        await context.Categories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        logger.LogInformation($"Categories seeded: {categories.Count} items");
    }

    private static async Task SeedBrandsAsync(AppDbContext context, ILogger logger)
    {
        if (await context.Brands.AnyAsync())
        {
            logger.LogInformation("Brands already exist, skipping");
            return;
        }

        logger.LogInformation("Seeding brands");

        var brands = new List<Brand>
        {
            new Brand { Name = "Nike" },
            new Brand { Name = "Adidas" },
            new Brand { Name = "Puma" },
            new Brand { Name = "Levis" },
            new Brand { Name = "Zara" }
        };

        await context.Brands.AddRangeAsync(brands);
        await context.SaveChangesAsync();

        logger.LogInformation($"Brands seeded: {brands.Count} items");
    }
}