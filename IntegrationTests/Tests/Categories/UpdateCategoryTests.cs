using Wardrobe.API.DTOs.Categories;

namespace IntegrationTests.Tests.Categories;

[Collection("IntegrationTests")]
public class UpdateCategoryTests
{
	private readonly IntegrationTestFixture _fixture;

	public UpdateCategoryTests(IntegrationTestFixture fixture)
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

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Original Cat {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var newName = $"Updated Cat {Guid.NewGuid().ToString("N")[..6]}";
		var dto = new UpdateCategoryDto { Name = newName };

		var response = await client.PutAsJsonAsync($"/api/categories/{category.Id}", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var updated = await response.Content.ReadFromJsonAsync<CategoryDto>();
		Assert.NotNull(updated);
		Assert.Equal(newName, updated.Name);
	}

	[Fact]
	public async Task Update_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Some Cat {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateCategoryDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync($"/api/categories/{category.Id}", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateCategoryDto { Name = "New Name" };

		var response = await client.PutAsJsonAsync("/api/categories/99999", dto);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}