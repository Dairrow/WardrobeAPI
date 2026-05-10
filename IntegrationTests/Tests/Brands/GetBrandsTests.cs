using Wardrobe.API.DTOs.Brands;

namespace IntegrationTests.Tests.Brands;

[Collection("IntegrationTests")]
public class GetBrandsTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetBrandsTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithValidToken_ReturnsOkWithBrands()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var brand1Name = $"Brand_{Guid.NewGuid().ToString("N")[..6]}";
		var brand2Name = $"Brand_{Guid.NewGuid().ToString("N")[..6]}";

		await TestDataSeeder.SeedBrandAsync(context, brand1Name);
		await TestDataSeeder.SeedBrandAsync(context, brand2Name);

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/brands");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var brands = await response.Content.ReadFromJsonAsync<List<BrandDto>>();
		Assert.NotNull(brands);
		Assert.True(brands.Count >= 2);
		Assert.Contains(brands, b => b.Name == brand1Name);
		Assert.Contains(brands, b => b.Name == brand2Name);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/brands");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetAll_WithExpiredToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var expiredToken = JwtTokenGenerator.GenerateExpiredToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

		var response = await client.GetAsync("/api/brands");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithValidId_ReturnsBrand()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var brandName = $"Puma_{Guid.NewGuid().ToString("N")[..6]}";
		var brand = await TestDataSeeder.SeedBrandAsync(context, brandName);

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/brands/{brand.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var brandDto = await response.Content.ReadFromJsonAsync<BrandDto>();
		Assert.NotNull(brandDto);
		Assert.Equal(brand.Id, brandDto.Id);
		Assert.Equal(brandName, brandDto.Name);
	}

	[Fact]
	public async Task GetById_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/brands/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/brands/1");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}
}