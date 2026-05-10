using System.Net.Http.Headers;
using Wardrobe.API.DTOs.ClothingItems;

namespace IntegrationTests.Tests.ClothingItems;

[Collection("IntegrationTests")]
public class CreateClothingItemTests
{
	private readonly IntegrationTestFixture _fixture;

	public CreateClothingItemTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Create_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"RegularUser_{Guid.NewGuid().ToString("N")[..6]}",
			$"regular_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"User");

		var token = JwtTokenGenerator.GenerateToken(user.Id, user.Email, "User");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var formData = new MultipartFormDataContent
		{
			{ new StringContent("Test Item"), "Name" },
			{ new StringContent("10.00"), "Price" },
			{ new StringContent("1"), "CategoryId" },
			{ new StringContent("1"), "BrandId" }
		};

		var response = await client.PostAsync("/api/clothingitems", formData);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var formData = new MultipartFormDataContent
		{
			{ new StringContent("Test"), "Name" },
			{ new StringContent("10.00"), "Price" },
			{ new StringContent("1"), "CategoryId" },
			{ new StringContent("1"), "BrandId" }
		};

		var response = await client.PostAsync("/api/clothingitems", formData);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithZeroPrice_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var adminUser = await TestDataSeeder.SeedUserAsync(
			context,
			$"Admin_{Guid.NewGuid().ToString("N")[..6]}",
			$"adm_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!",
			"Admin");

		var token = JwtTokenGenerator.GenerateToken(adminUser.Id, adminUser.Email, "Admin");
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var formData = new MultipartFormDataContent
		{
			{ new StringContent("Zero Price Item"), "Name" },
			{ new StringContent("0"), "Price" },
			{ new StringContent("1"), "CategoryId" },
			{ new StringContent("1"), "BrandId" }
		};

		var response = await client.PostAsync("/api/clothingitems", formData);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}