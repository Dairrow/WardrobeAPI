using Microsoft.Extensions.DependencyInjection;
using Wardrobe.Repositories.Implementations;
using Wardrobe.Repositories.Interfaces;

namespace Wardrobe.Repositories.DependencyInjection;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();

        services.AddScoped<IRoleRepository, RoleRepository>();

        services.AddScoped<IBrandRepository, BrandRepository>();

        services.AddScoped<IClothingItemRepository, ClothingItemRepository>();

        services.AddScoped<IOutfitRepository, OutfitRepository>();

        services.AddScoped<IOutfitItemRepository, OutfitItemRepository>();

        return services;
    }
}