using FluentAssertions;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.UnitTests.Services;

public class AuthServiceTests
{
	private readonly Mock<IUserRepository> _userRepositoryMock;
	private readonly Mock<IRoleRepository> _roleRepositoryMock;
	private readonly Mock<IJwtService> _jwtServiceMock;
	private readonly AuthService _authService;

	public AuthServiceTests()
	{
		_userRepositoryMock = new Mock<IUserRepository>();
		_roleRepositoryMock = new Mock<IRoleRepository>();
		_jwtServiceMock = new Mock<IJwtService>();
		_authService = new AuthService(
			_userRepositoryMock.Object,
			_roleRepositoryMock.Object,
			_jwtServiceMock.Object);
	}

	#region Register Tests

	[Fact]
	public async Task Register_Should_CreateUser_When_DataIsValid()
	{
		var username = "testuser";
		var email = "test@example.com";
		var password = "password123";
		var role = new Role { Id = 1, Name = "User" };

		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(email))
			.ReturnsAsync((User?)null);

		_roleRepositoryMock
			.Setup(x => x.GetByNameAsync("User"))
			.ReturnsAsync(role);

		_userRepositoryMock
			.Setup(x => x.AddAsync(It.IsAny<User>()))
			.ReturnsAsync((User u) => u);

		var result = await _authService.RegisterAsync(username, email, password);

		result.Should().NotBeNull();
		result.Username.Should().Be(username);
		result.Email.Should().Be(email);
		result.RoleId.Should().Be(role.Id);
		_userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
	}

	[Fact]
	public async Task Register_Should_ThrowConflictException_When_UserAlreadyExists()
	{
		var email = "existing@example.com";
		var existingUser = new User { Id = 1, Email = email };

		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(email))
			.ReturnsAsync(existingUser);

		var action = async () =>
			await _authService.RegisterAsync("test", email, "password123");

		await action.Should().ThrowAsync<ConflictException>()
			.WithMessage("User already exists");
	}

	[Fact]
	public async Task Register_Should_ThrowNotFoundException_When_RoleNotFound()
	{
		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
			.ReturnsAsync((User?)null);

		_roleRepositoryMock
			.Setup(x => x.GetByNameAsync("User"))
			.ReturnsAsync((Role?)null);

		var action = async () =>
			await _authService.RegisterAsync("test", "test@test.com", "password123");

		await action.Should().ThrowAsync<NotFoundException>()
			.WithMessage("User role not found");
	}

	#endregion

	#region Login Tests

	[Fact]
	public async Task Login_Should_ReturnToken_When_CredentialsAreValid()
	{
		var email = "test@example.com";
		var password = "password123";
		var user = new User
		{
			Id = 1,
			Email = email,
			PasswordHash = PasswordHasher.Hash(password),
			Role = new Role { Name = "User" }
		};
		var expectedToken = "valid.jwt.token";

		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(email))
			.ReturnsAsync(user);

		_jwtServiceMock
			.Setup(x => x.GenerateToken(user))
			.Returns(expectedToken);

		var token = await _authService.LoginAsync(email, password);

		token.Should().Be(expectedToken);
		_jwtServiceMock.Verify(x => x.GenerateToken(user), Times.Once);
	}

	[Fact]
	public async Task Login_Should_ThrowUnauthorizedException_When_UserNotFound()
	{
		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
			.ReturnsAsync((User?)null);

		var action = async () =>
			await _authService.LoginAsync("nonexistent@test.com", "password123");

		await action.Should().ThrowAsync<UnauthorizedException>()
			.WithMessage("Invalid credentials");
	}

	[Fact]
	public async Task Login_Should_ThrowUnauthorizedException_When_PasswordIsWrong()
	{
		var email = "test@example.com";
		var user = new User
		{
			Id = 1,
			Email = email,
			PasswordHash = PasswordHasher.Hash("correct_password"),
			Role = new Role { Name = "User" }
		};

		_userRepositoryMock
			.Setup(x => x.GetByEmailAsync(email))
			.ReturnsAsync(user);

		var action = async () =>
			await _authService.LoginAsync(email, "wrong_password");

		await action.Should().ThrowAsync<UnauthorizedException>()
			.WithMessage("Invalid credentials");
	}

	#endregion
}

// Временный хелпер для тестов (если PasswordHasher недоступен)
internal static class PasswordHasher
{
	public static string Hash(string password)
	{
		using var sha256 = System.Security.Cryptography.SHA256.Create();
		var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
		return Convert.ToBase64String(bytes);
	}
}