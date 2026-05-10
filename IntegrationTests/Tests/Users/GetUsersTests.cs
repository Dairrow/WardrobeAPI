using Wardrobe.API.DTOs.Users;

namespace IntegrationTests.Tests.Users;

[Collection("IntegrationTests")]
public class GetUsersTests
{
	private readonly IntegrationTestFixture _fixture;

	public GetUsersTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task GetAll_WithAdminToken_ReturnsOkWithUsers()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		await TestDataSeeder.SeedUserAsync(context, "UserOne", $"user1_{Guid.NewGuid().ToString("N")[..6]}@test.com", "Pass123!");
		await TestDataSeeder.SeedUserAsync(context, "UserTwo", $"user2_{Guid.NewGuid().ToString("N")[..6]}@test.com", "Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/users");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
		Assert.NotNull(users);
		Assert.True(users.Count >= 2);
		Assert.Contains(users, u => u.Username == "UserOne");
		Assert.Contains(users, u => u.Username == "UserTwo");
	}

	[Fact]
	public async Task GetAll_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/users");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task GetAll_WithoutToken_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var response = await client.GetAsync("/api/users");

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithAdminTokenAndValidId_ReturnsUser()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var email = $"findme_{Guid.NewGuid().ToString("N")[..6]}@test.com";
		var user = await TestDataSeeder.SeedUserAsync(context, "FindMe", email, "Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync($"/api/users/{user.Id}");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
		Assert.NotNull(userDto);
		Assert.Equal(user.Id, userDto.Id);
		Assert.Equal("FindMe", userDto.Username);
		Assert.Equal(email, userDto.Email);
		Assert.NotEqual("User", userDto.RoleName);
	}

	[Fact]
	public async Task GetById_WithAdminTokenAndNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/users/99999");

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task GetById_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var response = await client.GetAsync("/api/users/1");

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}
}