using Wardrobe.API.DTOs.Users;

namespace IntegrationTests.Tests.Users;

[Collection("IntegrationTests")]
public class CreateUserTests
{
	private readonly IntegrationTestFixture _fixture;

	public CreateUserTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}


	[Fact]
	public async Task Create_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateUserDto
		{
			Username = "testuser",
			Email = "test@test.com",
			Password = "Password123!",
			RoleId = 1
		};

		var response = await client.PostAsJsonAsync("/api/users", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithDuplicateEmail_ReturnsConflict()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

		var email = $"dup_{Guid.NewGuid().ToString("N")[..6]}@test.com";
		await TestDataSeeder.SeedUserAsync(context, "Existing", email, "Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateUserDto
		{
			Username = "NewUser",
			Email = email,
			Password = "Password123!",
			RoleId = userRole.Id
		};

		var response = await client.PostAsJsonAsync("/api/users", dto);

		Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithInvalidEmail_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateUserDto
		{
			Username = "testuser",
			Email = "not-an-email",
			Password = "Password123!",
			RoleId = userRole.Id
		};

		var response = await client.PostAsJsonAsync("/api/users", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Create_WithShortPassword_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new CreateUserDto
		{
			Username = "testuser",
			Email = $"test_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			Password = "12345",
			RoleId = userRole.Id
		};

		var response = await client.PostAsJsonAsync("/api/users", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}