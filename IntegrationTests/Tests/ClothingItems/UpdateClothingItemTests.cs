using System.Net.Http.Headers;
using Wardrobe.API.DTOs.ClothingItems;

namespace IntegrationTests.Tests.ClothingItems;

[Collection("IntegrationTests")]
public class UpdateClothingItemTests
{
	private readonly IntegrationTestFixture _fixture;

	public UpdateClothingItemTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Update_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"Regular_{Guid.NewGuid().ToString("N")[..6]}",
			$"reg_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"User");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var formData = new MultipartFormDataContent
		{
			{ new StringContent("Name"), "Name" },
			{ new StringContent("10.00"), "Price" }
		};

		var response = await client.PutAsync("/api/clothingitems/1", formData);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var adminUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"NotFoundAdmin_{Guid.NewGuid().ToString("N")[..6]}",
			$"nfadm_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"Admin");

		var token = JwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Email, "Admin");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var formData = new MultipartFormDataContent
		{
			{ new StringContent("Test"), "Name" },
			{ new StringContent("10.00"), "Price" }
		};

		var response = await client.PutAsync("/api/clothingitems/99999", formData);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}