using Wardrobe.API.DTOs.Brands;

namespace IntegrationTests.Tests.Brands;

[Collection("IntegrationTests")]
public class UpdateBrandTests
{
	private readonly IntegrationTestFixture _fixture;

	public UpdateBrandTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Update_WithValidDataAndAdminToken_ReturnsOk()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var originalName = $"Original {Guid.NewGuid().ToString("N")[..6]}";
		var brand = await TestDataSeeder.SeedBrandAsync(context, originalName);

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var updatedName = $"Updated {Guid.NewGuid().ToString("N")[..6]}";
		var dto = new UpdateBrandDto
		{
			Name = updatedName
		};

		var response = await client.PutAsJsonAsync($"/api/brands/{brand.Id}", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var updatedBrand = await response.Content.ReadFromJsonAsync<BrandDto>();
		Assert.NotNull(updatedBrand);
		Assert.Equal(updatedName, updatedBrand.Name);
	}

	[Fact]
	public async Task Update_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var brand = await TestDataSeeder.SeedBrandAsync(context, $"Some Brand {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateBrandDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync($"/api/brands/{brand.Id}", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new UpdateBrandDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync("/api/brands/1", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateBrandDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync("/api/brands/99999", dto);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithEmptyName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var brand = await TestDataSeeder.SeedBrandAsync(context, $"Brand To Update {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateBrandDto { Name = "" };

		var response = await client.PutAsJsonAsync($"/api/brands/{brand.Id}", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithTooLongName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var brand = await TestDataSeeder.SeedBrandAsync(context, $"Brand Long {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateBrandDto { Name = new string('Y', 101) };

		var response = await client.PutAsJsonAsync($"/api/brands/{brand.Id}", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}