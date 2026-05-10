namespace IntegrationTests.Tests.ClothingItems;

[Collection("IntegrationTests")]
public class DeleteClothingItemTests
{
	private readonly IntegrationTestFixture _fixture;

	public DeleteClothingItemTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Delete_WithValidIdAndAdminToken_ReturnsNoContent()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var adminUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"AdminDel_{Guid.NewGuid().ToString("N")[..6]}",
			$"admdel_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"Admin");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var item = await TestDataSeeder.SeedClothingItemAsync(
			context, adminUser.Id, category.Id, brand.Id, $"Delete Me {Guid.NewGuid().ToString("N")[..6]}", 10.00m);

		var token = JwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Email, "Admin");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/clothingitems/{item.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var deleted = await context.ClothingItems.FindAsync(item.Id);
		Assert.NotNull(deleted);
	}

	[Fact]
	public async Task Delete_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"User_{Guid.NewGuid().ToString("N")[..6]}",
			$"user_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"User");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync("/api/clothingitems/1");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var adminUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"AdminNF_{Guid.NewGuid().ToString("N")[..6]}",
			$"nf_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"Admin");

		var token = JwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Email, "Admin");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync("/api/clothingitems/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.DeleteAsync("/api/clothingitems/1");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}
}