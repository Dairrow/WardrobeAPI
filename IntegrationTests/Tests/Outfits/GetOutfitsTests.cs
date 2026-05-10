using Wardrobe.API.DTOs.Outfits;

namespace IntegrationTests.Tests.Outfits;

[Collection("IntegrationTests")]
public class GetOutfitsTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetOutfitsTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithValidToken_ReturnsOnlyUserOutfits()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var user1 = await TestDataSeeder.SeedUserAsync(
			context,
			$"User1_{Guid.NewGuid().ToString("N")[..6]}",
			$"u1_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var user2 = await TestDataSeeder.SeedUserAsync(
			context,
			$"User2_{Guid.NewGuid().ToString("N")[..6]}",
			$"u2_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		await TestDataSeeder.SeedOutfitAsync(context, user1.Id, "User1 Outfit");
		await TestDataSeeder.SeedOutfitAsync(context, user2.Id, "User2 Outfit");

		var token = JwtTokenGenerator.GenerateToken(user1.Id, user1.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/outfits");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var outfits = await response.Content.ReadFromJsonAsync<List<OutfitDto>>();
		Assert.NotNull(outfits);
		Assert.Single(outfits);
		Assert.Equal("User1 Outfit", outfits[0].Name);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/outfits");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithValidIdAndOwnerToken_ReturnsOutfit()
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

		var outfit = await TestDataSeeder.SeedOutfitAsync(context, user.Id, "My Outfit");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/outfits/{outfit.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var outfitDto = await response.Content.ReadFromJsonAsync<OutfitDto>();
		Assert.NotNull(outfitDto);
		Assert.Equal(outfit.Id, outfitDto.Id);
		Assert.Equal("My Outfit", outfitDto.Name);
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
			$"own_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var otherUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"Other_{Guid.NewGuid().ToString("N")[..6]}",
			$"oth_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var outfit = await TestDataSeeder.SeedOutfitAsync(context, owner.Id, "Owners Outfit");

		var token = JwtTokenGenerator.GenerateToken(otherUser.Id, otherUser.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/outfits/{outfit.Id}");

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
			$"u_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/outfits/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}