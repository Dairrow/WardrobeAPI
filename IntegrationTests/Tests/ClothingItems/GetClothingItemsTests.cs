using Wardrobe.API.DTOs.ClothingItems;

namespace IntegrationTests.Tests.ClothingItems;

[Collection("IntegrationTests")]
public class GetClothingItemsTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetClothingItemsTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithValidToken_ReturnsOnlyUserItems()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var user1 = await TestDataSeeder.SeedUserAsync(
			context,
			$"User1_{Guid.NewGuid().ToString("N")[..6]}",
			$"user1_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var user2 = await TestDataSeeder.SeedUserAsync(
			context,
			$"User2_{Guid.NewGuid().ToString("N")[..6]}",
			$"user2_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		await TestDataSeeder.SeedClothingItemAsync(
			context, user1.Id, category.Id, brand.Id, "User1 Item", 10.99m);

		await TestDataSeeder.SeedClothingItemAsync(
			context, user2.Id, category.Id, brand.Id, "User2 Item", 20.99m);

		var token = JwtTokenGenerator.GenerateToken(user1.Id, user1.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/clothingitems");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var items = await response.Content.ReadFromJsonAsync<List<ClothingItemDto>>();
		Assert.NotNull(items);
		Assert.Equal("User1 Item", items[0].Name);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/clothingitems");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithValidIdAndOwnerToken_ReturnsItem()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"Owner_{Guid.NewGuid().ToString("N")[..6]}",
			$"owner_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var item = await TestDataSeeder.SeedClothingItemAsync(
			context, user.Id, category.Id, brand.Id, "My Item", 50.00m);

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/clothingitems/{item.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var itemDto = await response.Content.ReadFromJsonAsync<ClothingItemDto>();
		Assert.NotNull(itemDto);
		Assert.Equal(item.Id, itemDto.Id);
		Assert.Equal("My Item", itemDto.Name);
		Assert.Equal(50.00m, itemDto.Price);
		Assert.Equal(category.Name, itemDto.CategoryName);
		Assert.Equal(brand.Name, itemDto.BrandName);
	}

	[Fact]
	public async Task GetById_WithOtherUserToken_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var owner = await TestDataSeeder.SeedUserAsync(
			context,
			$"Owner_{Guid.NewGuid().ToString("N")[..6]}",
			$"owner_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var otherUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"Other_{Guid.NewGuid().ToString("N")[..6]}",
			$"other_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var item = await TestDataSeeder.SeedClothingItemAsync(
			context, owner.Id, category.Id, brand.Id, "Owners Item", 30.00m);

		var token = JwtTokenGenerator.GenerateToken(otherUser.Id, otherUser.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/clothingitems/{item.Id}");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithNonExistentId_ReturnsNotFound()
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
			"Pass123!");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/clothingitems/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}