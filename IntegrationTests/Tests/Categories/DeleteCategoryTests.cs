namespace IntegrationTests.Tests.Categories;

[Collection("IntegrationTests")]
public class DeleteCategoryTests
{
	private readonly IntegrationTestFixture _fixture;

	public DeleteCategoryTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Delete_WithValidIdAndAdminToken_ReturnsNoContent()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Delete Me {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/categories/{category.Id}");

		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

		var deleted = await context.Categories.FindAsync(category.Id);
		Assert.NotNull(deleted);
	}

	[Fact]
	public async Task Delete_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var category = await TestDataSeeder.SeedCategoryAsync(
			context,
			$"Protected Cat {Guid.NewGuid().ToString("N")[..6]}");

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync($"/api/categories/{category.Id}");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Delete_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.DeleteAsync("/api/categories/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}
}