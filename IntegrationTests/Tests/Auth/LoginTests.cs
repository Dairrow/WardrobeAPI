using Wardrobe.API.DTOs.Auth;

namespace IntegrationTests.Tests.Auth;

[Collection("IntegrationTests")]
public class LoginTests
{
	private readonly IntegrationTestFixture _fixture;

	public LoginTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Login_WithValidCredentials_ReturnsToken()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com";

		await TestDataSeeder.SeedRolesAsync(context);
		await TestDataSeeder.SeedUserAsync(
			context,
			email: email,
			password: "CorrectPassword1!");

		var dto = new LoginDto
		{
			Email = email,
			Password = "CorrectPassword1!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/login", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

		Assert.NotNull(authResponse);
		Assert.False(string.IsNullOrWhiteSpace(authResponse.Token));
	}

	[Fact]
	public async Task Login_WithWrongPassword_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com";

		await TestDataSeeder.SeedRolesAsync(context);
		await TestDataSeeder.SeedUserAsync(
			context,
			email: email,
			password: "RightPassword1!");

		var dto = new LoginDto
		{
			Email = email,
			Password = "WrongPassword1!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/login", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new LoginDto
		{
			Email = $"{Guid.NewGuid().ToString("N")[..8]}@nonexistent.com",
			Password = "SomePassword1!"
		};

		var response = await client.PostAsJsonAsync("/api/auth/login", dto);

		Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
	}

	[Fact]
	public async Task Login_WithEmptyBody_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var emptyJson = "{}";
		var content = new StringContent(emptyJson, Encoding.UTF8, "application/json");

		var response = await client.PostAsync("/api/auth/login", content);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Login_WithoutPassword_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var dto = new { Email = "test@test.com" };

		var response = await client.PostAsJsonAsync("/api/auth/login", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Login_IssuedToken_CanAccessProtectedEndpoint()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		var email = $"{Guid.NewGuid().ToString("N")[..8]}@test.com";

		await TestDataSeeder.SeedRolesAsync(context);
		await TestDataSeeder.SeedUserAsync(
			context,
			username: Guid.NewGuid().ToString("N")[..10],
			email: email,
			password: "AccessGranted1!",
			roleName: "Admin");

		var loginDto = new LoginDto
		{
			Email = email,
			Password = "AccessGranted1!"
		};

		var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginDto);
		var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDto>();

		client.DefaultRequestHeaders.Authorization =
			new AuthenticationHeaderValue("Bearer", authResponse!.Token);

		var response = await client.GetAsync("/api/brands");

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);
	}
}