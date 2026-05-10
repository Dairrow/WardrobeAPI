namespace IntegrationTests.Tests.Users;

[Collection("IntegrationTests")]
public class DeleteUserTests
{
	private readonly IntegrationTestFixture _fixture;

	public DeleteUserTests(IntegrationTestFixture fixture)
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
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"DeleteMe_{Guid.NewGuid().ToString("N")[..6]}",
			$"delete_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/users/{user.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var deletedUser = await context.Users.FindAsync(user.Id);
		Assert.NotNull(deletedUser);
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
			$"Protected_{Guid.NewGuid().ToString("N")[..6]}",
			$"protected_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/users/{user.Id}");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.DeleteAsync("/api/users/1");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync("/api/users/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WhenUserHasClothingItems_CascadesDeletion()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"CascadeUser_{Guid.NewGuid().ToString("N")[..6]}",
			$"cascade_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		await TestDataSeeder.SeedClothingItemAsync(
			context,
			user.Id,
			category.Id,
			brand.Id,
			"Item To Cascade");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/users/{user.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var deletedUser = await context.Users.FindAsync(user.Id);
		Assert.NotNull(deletedUser);

		var remainingItems = await context.ClothingItems
			.Where(ci => ci.UserId == user.Id)
			.ToListAsync();
		Assert.Empty(remainingItems);
	}
}