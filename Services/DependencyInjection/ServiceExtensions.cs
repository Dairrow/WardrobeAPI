using Microsoft.Extensions.DependencyInjection;
using Wardrobe.Services.Implementations;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(
        this IServiceCollection services)
    {

        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<IRoleService, RoleService>();

        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ICategoryService, CategoryService>();

        services.AddScoped<IFileService,FileService>();

        services.AddScoped<IBrandService, BrandService>();

        services.AddScoped<IClothingItemService, ClothingItemService>();

        services.AddScoped<IOutfitService, OutfitService>();

        services.AddScoped<IOutfitItemService, OutfitItemService>();


        return services;
    }
}