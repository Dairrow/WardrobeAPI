using Wardrobe.API.DTOs.Outfits;

namespace IntegrationTests.Tests.Outfits;

[Collection("IntegrationTests")]
public class UpdateOutfitTests
{
	private readonly IntegrationTestFixture _fixture;

	public UpdateOutfitTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Update_WithValidDataAndToken_ReturnsOk()
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

		var outfit = await TestDataSeeder.SeedOutfitAsync(
			context,
			user.Id,
			$"Original {Guid.NewGuid().ToString("N")[..6]}");

		var newName = $"Updated {Guid.NewGuid().ToString("N")[..6]}";

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateOutfitDto { Name = newName };

		var response = await client.PutAsJsonAsync($"/api/outfits/{outfit.Id}", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var updatedDto = await response.Content.ReadFromJsonAsync<OutfitDto>();
		Assert.NotNull(updatedDto);
		Assert.Equal(newName, updatedDto.Name);
	}

	[Fact]
	public async Task Update_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new UpdateOutfitDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync("/api/outfits/1", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithNonExistentId_ReturnsNotFound()
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

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateOutfitDto { Name = "Updated Name" };

		var response = await client.PutAsJsonAsync("/api/outfits/99999", dto);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}