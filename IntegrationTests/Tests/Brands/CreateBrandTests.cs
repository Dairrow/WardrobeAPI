using Wardrobe.API.DTOs.Brands;

namespace IntegrationTests.Tests.Brands;

[Collection("IntegrationTests")]
public class CreateBrandTests
{
	private readonly IntegrationTestFixture _fixture;

	public CreateBrandTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Create_WithValidDataAndAdminToken_ReturnsCreated()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var brandName = $"Test Brand {Guid.NewGuid().ToString("N")[..6]}";
		var dto = new CreateBrandDto
		{
			Name = brandName
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var brandDto = await response.Content.ReadFromJsonAsync<BrandDto>();
		Assert.NotNull(brandDto);
		Assert.NotEqual(0, brandDto.Id);
		Assert.Equal(brandName, brandDto.Name);
	}

	[Fact]
	public async Task Create_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateBrandDto
		{
			Name = "Test Brand"
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new CreateBrandDto
		{
			Name = "Test Brand"
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithDuplicateName_ReturnsConflict()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var duplicateName = $"Duplicate {Guid.NewGuid().ToString("N")[..6]}";
		await TestDataSeeder.SeedBrandAsync(context, duplicateName);

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateBrandDto
		{
			Name = duplicateName
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithEmptyName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateBrandDto
		{
			Name = ""
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithTooShortName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateBrandDto
		{
			Name = "A"
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithTooLongName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateBrandDto
		{
			Name = new string('X', 101)
		};

		var response = await client.PostAsJsonAsync("/api/brands", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}