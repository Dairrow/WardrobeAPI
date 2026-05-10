using Wardrobe.API.DTOs.Auth;

namespace IntegrationTests.Tests.Auth;

[Collection("IntegrationTests")]
public class RegisterTests
{
	private readonly IntegrationTestFixture _fixture;

	public RegisterTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Register_WithValidData_ReturnsOk()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var dto = new RegisterDto
		{
			Username = Guid.NewGuid().ToString("N")[..10],
			Email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com",
			Password = "Password123!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/register", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}

	[Fact]
	public async Task Register_WithDuplicateEmail_ReturnsConflict()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com";
		await TestDataSeeder.SeedUserAsync(context, email: email);

		var dto = new RegisterDto
		{
			Username = Guid.NewGuid().ToString("N")[..10],
			Password = "Password123!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/register", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_WithInvalidEmail_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var dto = new RegisterDto
		{
			Username = Guid.NewGuid().ToString("N")[..10],
			Email = "not-an-email",
			Password = "Password123!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/register", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_WithShortPassword_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var dto = new RegisterDto
		{
			Username = Guid.NewGuid().ToString("N")[..10],
			Email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com",
			Password = "12345"
		};

		var response = await client.PostAsJsonAsync("/api/auth/register", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_WithEmptyBody_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var emptyJson = "{}";
		var content = new StringContent(emptyJson, Encoding.UTF8, "application/json");

		var response = await client.PostAsync("/api/auth/register", content);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Register_WithValidData_CreatesUserInDatabase()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);

		var email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com";
		var username = Guid.NewGuid().ToString("N")[..10];
		var password = "SecurePass789!";

		var dto = new RegisterDto
		{
			Username = username,
			Email = email,
			Password = password
		};

		await client.PostAsJsonAsync("/api/auth/register", dto);

		var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);

		Assert.NotNull(user);
		Assert.Equal(username, user.Username);
		Assert.Equal(PasswordHasher.Hash(password), user.PasswordHash);
		Assert.NotEqual(0, user.Id);
		Assert.NotEqual(default, user.CreatedAt);
	}
}