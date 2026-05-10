using Wardrobe.API.DTOs.Roles;

namespace IntegrationTests.Tests.Roles;

[Collection("IntegrationTests")]
public class GetRolesTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetRolesTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithAdminToken_ReturnsOkWithRoles()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/roles");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var roles = await response.Content.ReadFromJsonAsync<List<RoleDto>>();
		Assert.NotNull(roles);
		Assert.True(roles.Count >= 2);
		Assert.Contains(roles, r => r.Name == "Admin");
		Assert.Contains(roles, r => r.Name == "User");
	}

	[Fact]
	public async Task GetAll_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/roles");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/roles");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetAll_WithExpiredToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var expiredToken = JwtTokenGenerator.GenerateExpiredToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", expiredToken);

		var response = await client.GetAsync("/api/roles");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithAdminTokenAndValidId_ReturnsRole()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "User");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/roles/{userRole!.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var roleDto = await response.Content.ReadFromJsonAsync<RoleDto>();
		Assert.NotNull(roleDto);
		Assert.Equal(userRole.Id, roleDto.Id);
		Assert.Equal("User", roleDto.Name);
	}

	[Fact]
	public async Task GetById_WithAdminTokenAndNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/roles/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/roles/1");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}