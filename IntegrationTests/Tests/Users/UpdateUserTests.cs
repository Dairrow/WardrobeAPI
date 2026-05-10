using Wardrobe.API.DTOs.Users;

namespace IntegrationTests.Tests.Users;

[Collection("IntegrationTests")]
public class UpdateUserTests
{
	private readonly IntegrationTestFixture _fixture;

	public UpdateUserTests(IntegrationTestFixture fixture)
	{
		_fixture = fixture;
	}

	[Fact]
	public async Task Update_WithValidDataAndAdminToken_ReturnsOk()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstAsync(r => r.Name == "User");
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"Original_{Guid.NewGuid().ToString("N")[..6]}",
			$"orig_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var newUsername = $"Updated_{Guid.NewGuid().ToString("N")[..6]}";
		var newEmail = $"updated_{Guid.NewGuid().ToString("N")[..6]}@test.com";

		var dto = new UpdateUserDto
		{
			Username = newUsername,
			Email = newEmail,
			RoleId = userRole.Id
		};

		var response = await client.PutAsJsonAsync($"/api/users/{user.Id}", dto);

		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var updatedUser = await response.Content.ReadFromJsonAsync<UserDto>();
		Assert.NotNull(updatedUser);
		Assert.Equal(newUsername, updatedUser.Username);
		Assert.Equal(newEmail, updatedUser.Email);
	}

	[Fact]
	public async Task Update_WithUserToken_ReturnsForbidden()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"SomeUser_{Guid.NewGuid().ToString("N")[..6]}",
			$"some_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateUserToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateUserDto
		{
			Username = "NewName",
			Email = "new@test.com",
			RoleId = 1
		};

		var response = await client.PutAsJsonAsync($"/api/users/{user.Id}", dto);

		Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithNonExistentId_ReturnsNotFound()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateUserDto
		{
			Username = "NewName",
			Email = "new@test.com",
			RoleId = 1
		};

		var response = await client.PutAsJsonAsync("/api/users/99999", dto);

		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithDuplicateEmail_ReturnsInternalServerError()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

		var existingEmail = $"taken_{Guid.NewGuid().ToString("N")[..6]}@test.com";
		await TestDataSeeder.SeedUserAsync(context, "User1", existingEmail, "Pass123!");
		var userToUpdate = await TestDataSeeder.SeedUserAsync(
			context,
			$"User2_{Guid.NewGuid().ToString("N")[..6]}",
			$"free_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateUserDto
		{
			Username = "UpdatedName",
			RoleId = userRole.Id
		};

		var response = await client.PutAsJsonAsync($"/api/users/{userToUpdate.Id}", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}

	[Fact]
	public async Task Update_WithInvalidEmail_ReturnsBadRequest()
	{
		await using var factory = await _fixture.CreateFactoryAsync();
		var client = factory.CreateClient();

		using var scope = factory.Services.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

		await TestDataSeeder.SeedRolesAsync(context);
		var user = await TestDataSeeder.SeedUserAsync(
			context,
			$"ValidUser_{Guid.NewGuid().ToString("N")[..6]}",
			$"valid_{Guid.NewGuid().ToString("N")[..6]}@test.com",
			"Pass123!");

		var token = JwtTokenGenerator.GenerateAdminToken();
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

		var dto = new UpdateUserDto
		{
			Username = "ValidName",
			Email = "not-an-email",
			RoleId = 1
		};

		var response = await client.PutAsJsonAsync($"/api/users/{user.Id}", dto);

		Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
	}
}