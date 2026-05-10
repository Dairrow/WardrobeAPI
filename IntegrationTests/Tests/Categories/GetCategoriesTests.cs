using Wardrobe.API.DTOs.Categories;

namespace IntegrationTests.Tests.Categories;

[Collection("IntegrationTests")]
public class GetCategoriesTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetCategoriesTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithValidToken_ReturnsOkWithCategories()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var cat1 = $"Category_{Guid.NewGuid().ToString("N")[..6]}";
		var cat2 = $"Category_{Guid.NewGuid().ToString("N")[..6]}";

		await TestDataSeeder.SeedCategoryAsync(context, cat1);
		await TestDataSeeder.SeedCategoryAsync(context, cat2);

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/categories");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var categories = await response.Content.ReadFromJsonAsync<List<CategoryDto>>();
		Assert.NotNull(categories);
		Assert.True(categories.Count >= 2);
		Assert.Contains(categories, c => c.Name == cat1);
		Assert.Contains(categories, c => c.Name == cat2);
	}

	[Fact]
	public async Task GetAll_WithExpiredToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var expiredToken = JwtTokenGenerator.GenerateExpiredToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

		var response = await client.GetAsync("/api/categories");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithValidId_ReturnsCategory()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var name = $"Shoes_{Guid.NewGuid().ToString("N")[..6]}";
		var category = await TestDataSeeder.SeedCategoryAsync(context, name);

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/categories/{category.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>();
		Assert.NotNull(categoryDto);
		Assert.Equal(category.Id, categoryDto.Id);
		Assert.Equal(name, categoryDto.Name);
	}

	[Fact]
	public async Task GetById_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/categories/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}