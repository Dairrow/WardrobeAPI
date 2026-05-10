using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Infrastructure;

public static class TestDataSeeder
{
	public static async Task SeedRolesAsync(AppDbContext context)
	{
		if (!await context.Roles.AnyAsync(r => r.Name == "Admin"))
		{
			await context.Roles.AddAsync(new Role { Name = "Admin" });
		}

		if (!await context.Roles.AnyAsync(r => r.Name == "User"))
		{
			await context.Roles.AddAsync(new Role { Name = "User" });
		}

		await context.SaveChangesAsync();
	}

	public static async Task<User> SeedUserAsync(
		AppDbContext context,
		string username = "testuser",
		string email = "user@wardrobe.local",
		string password = "User123!",
		string roleName = "User")
	{
		var role = await context.Roles.FirstAsync(r => r.Name == roleName);

		var user = new User
		{
			Username = username,
			Email = email,
			PasswordHash = PasswordHasher.Hash(password),
			RoleId = role.Id
		};

		await context.Users.AddAsync(user);
		await context.SaveChangesAsync();

		return user;
	}

	public static async Task<Category> SeedCategoryAsync(
		AppDbContext context,
		string name = "Test Category")
	{
		var category = new Category { Name = name };

		await context.Categories.AddAsync(category);
		await context.SaveChangesAsync();

		return category;
	}

	public static async Task<Brand> SeedBrandAsync(
		AppDbContext context,
		string name = "Test Brand")
	{
		var brand = new Brand { Name = name };

		await context.Brands.AddAsync(brand);
		await context.SaveChangesAsync();

		return brand;
	}

	public static async Task<ClothingItem> SeedClothingItemAsync(
		AppDbContext context,
		int userId,
		int categoryId,
		int brandId,
		string name = "Test Item",
		decimal price = 29.99m)
	{
		var item = new ClothingItem
		{
			Name = name,
			Price = price,
			CategoryId = categoryId,
			BrandId = brandId,
			UserId = userId
		};

		await context.ClothingItems.AddAsync(item);
		await context.SaveChangesAsync();

		return item;
	}

	public static async Task<Outfit> SeedOutfitAsync(
		AppDbContext context,
		int userId,
		string name = "Test Outfit")
	{
		var outfit = new Outfit
		{
			Name = name,
			UserId = userId
		};

		await context.Outfits.AddAsync(outfit);
		await context.SaveChangesAsync();

		return outfit;
	}

	public static async Task<OutfitItem> SeedOutfitItemAsync(
		AppDbContext context,
		int outfitId,
		int clothingItemId)
	{
		var outfitItem = new OutfitItem
		{
			OutfitId = outfitId,
			ClothingItemId = clothingItemId
		};

		await context.OutfitItems.AddAsync(outfitItem);
		await context.SaveChangesAsync();

		return outfitItem;
	}

	public static async Task<(User User, Category Category, Brand Brand)> SeedFullEnvironmentAsync(
		AppDbContext context,
		string username = "testuser",
		string email = "user@wardrobe.local",
		string password = "User123!",
		string categoryName = "Shoes",
		string brandName = "Nike")
	{
		await SeedRolesAsync(context);

		var user = await SeedUserAsync(context, username, email, password);
		var category = await SeedCategoryAsync(context, categoryName);
		var brand = await SeedBrandAsync(context, brandName);

		return (user, category, brand);
	}
}