using Wardrobe.API.DTOs.Categories;

namespace IntegrationTests.Tests.Categories;

[Collection("IntegrationTests")]
public class CreateCategoryTests
{
	private readonly IntegrationTestFixture _fixture;

	public CreateCategoryTests(IntegrationTestFixture fixture)
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

		var name = $"New Category {Guid.NewGuid().ToString("N")[..6]}";
		var dto = new CreateCategoryDto { Name = name };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);

		var categoryDto = await response.Content.ReadFromJsonAsync<CategoryDto>();
		Assert.NotNull(categoryDto);
		Assert.NotEqual(0, categoryDto.Id);
		Assert.Equal(name, categoryDto.Name);
	}

	[Fact]
	public async Task Create_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateCategoryDto { Name = "Test Category" };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithDuplicateName_ReturnsInternalServerError()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var duplicateName = $"Duplicate Cat {Guid.NewGuid().ToString("N")[..6]}";
		await TestDataSeeder.SeedCategoryAsync(context, duplicateName);

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateCategoryDto { Name = duplicateName };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithEmptyName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateCategoryDto { Name = "" };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithTooShortName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateCategoryDto { Name = "A" };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithTooLongName_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateCategoryDto { Name = new string('X', 101) };

		var response = await client.PostAsJsonAsync("/api/categories", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}