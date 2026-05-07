using Microsoft.EntityFrameworkCore;
using System;
using Wardrobe.Repositories.Context;
using Wardrobe.Services.DependencyInjection;

namespace Wardrobe.API.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("DefaultConnection");


        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));


        return services;
    }
}