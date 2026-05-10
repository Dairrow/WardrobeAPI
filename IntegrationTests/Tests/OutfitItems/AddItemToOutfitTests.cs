using Wardrobe.API.DTOs.OutfitItems;

namespace IntegrationTests.Tests.OutfitItems;

[Collection("IntegrationTests")]
public class AddItemToOutfitTests
{
	private readonly IntegrationTestFixture _fixture;

	public AddItemToOutfitTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task AddItem_WithValidData_ReturnsOk()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"User_{Guid.NewGuid().ToString("N")[..6]}",
			$"u_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var outfit = await TestDataSeeder.SeedOutfitAsync(
			context,
			user.Id,
			$"Outfit {Guid.NewGuid().ToString("N")[..6]}");

		var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
			context, user.Id, category.Id, brand.Id, $"New Item {Guid.NewGuid().ToString("N")[..6]}", 25.00m);

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateOutfitItemDto
		{
			OutfitId = outfit.Id,
			ClothingItemId = clothingItem.Id
		};

		var response = await client.PostAsJsonAsync("/api/outfititems", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var outfitItemDto = await response.Content.ReadFromJsonAsync<OutfitItemDto>();
		Assert.NotNull(outfitItemDto);
		Assert.Equal(outfit.Id, outfitItemDto.OutfitId);
		Assert.Equal(clothingItem.Id, outfitItemDto.ClothingItemId);
	}

	[Fact]
	public async Task AddItem_DuplicateItem_ReturnsConflict()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"User_{Guid.NewGuid().ToString("N")[..6]}",
			$"dup_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var outfit = await TestDataSeeder.SeedOutfitAsync(
			context,
			user.Id,
			$"Outfit {Guid.NewGuid().ToString("N")[..6]}");

		var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
			context, user.Id, category.Id, brand.Id, $"Duplicate Item {Guid.NewGuid().ToString("N")[..6]}", 30.00m);

		await TestDataSeeder.SeedOutfitItemAsync(context, outfit.Id, clothingItem.Id);

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateOutfitItemDto
		{
			OutfitId = outfit.Id,
			ClothingItemId = clothingItem.Id
		};

		var response = await client.PostAsJsonAsync("/api/outfititems", dto);

		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
	}

	[Fact]
	public async Task AddItem_WithOtherUsersClothingItem_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var owner = await TestDataSeeder.SeedUserAsync(
			context,
			$"Owner_{Guid.NewGuid().ToString("N")[..6]}",
			$"own_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var otherUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"Other_{Guid.NewGuid().ToString("N")[..6]}",
			$"oth_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var outfit = await TestDataSeeder.SeedOutfitAsync(
			context,
			owner.Id,
			$"Outfit {Guid.NewGuid().ToString("N")[..6]}");

		var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
			context, otherUser.Id, category.Id, brand.Id, $"Others Item {Guid.NewGuid().ToString("N")[..6]}", 40.00m);

		var token = JwtTokenGenerator.GenerateToken(owner.Id, owner.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateOutfitItemDto
		{
			OutfitId = outfit.Id,
			ClothingItemId = clothingItem.Id
		};

		var response = await client.PostAsJsonAsync("/api/outfititems", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task AddItem_WithNonExistentOutfit_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"User_{Guid.NewGuid().ToString("N")[..6]}",
			$"nf_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Cat_{Guid.NewGuid().ToString("N")[..6]}");

		var brand = await TestDataSeeder.SeedBrandAsync(
			context,
			$"Brand_{Guid.NewGuid().ToString("N")[..6]}");

		var clothingItem = await TestDataSeeder.SeedClothingItemAsync(
			context, user.Id, category.Id, brand.Id, $"Orphan Item {Guid.NewGuid().ToString("N")[..6]}", 10.00m);

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateOutfitItemDto
		{
			OutfitId = 99999,
			ClothingItemId = clothingItem.Id
		};

		var response = await client.PostAsJsonAsync("/api/outfititems", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task AddItem_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new CreateOutfitItemDto
		{
			OutfitId = 1,
			ClothingItemId = 1
		};

		var response = await client.PostAsJsonAsync("/api/outfititems", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}
}