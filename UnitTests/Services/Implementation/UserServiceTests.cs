using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;

namespace Wardrobe.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _repositoryMock;
    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _repositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<UserService>>();
        _userService = new UserService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetById_Should_ReturnUser_When_Exists()
    {
        var user = new User { Id = 1, Username = "john_doe", Email = "john@test.com" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        var result = await _userService.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Username.Should().Be("john_doe");
    }

    [Fact]
    public async Task GetByEmail_Should_ReturnUser_When_Exists()
    {
        var email = "john@test.com";
        var user = new User { Id = 1, Email = email };

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        var result = await _userService.GetByEmailAsync(email);

        result.Should().NotBeNull();
        result!.Email.Should().Be(email);
    }

    [Fact]
    public async Task Create_Should_ThrowConflictException_When_EmailExists()
    {
        var existingUser = new User { Id = 1, Email = "existing@test.com" };
        var newUser = new User { Email = "existing@test.com" };

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(newUser.Email))
            .ReturnsAsync(existingUser);

        var action = async () => await _userService.CreateAsync(newUser);

        await action.Should().ThrowAsync<ConflictException>()
            .WithMessage("User already exists");
    }

    [Fact]
    public async Task Create_Should_AddUser_When_EmailNotExists()
    {
        var user = new User
        {
            Username = "new_user",
            Email = "new@test.com",
            PasswordHash = "hash"
        };

        _repositoryMock
            .Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync((User?)null);

        _repositoryMock
            .Setup(x => x.AddAsync(user))
            .ReturnsAsync((User u) => { u.Id = 1; return u; });

        var result = await _userService.CreateAsync(user);

        result.Id.Should().Be(1);
        _loggerMock.VerifyLog(LogLevel.Information, "Creating user new@test.com");
    }

    [Fact]
    public async Task Update_Should_UpdateUserFields()
    {
        var existing = new User
        {
            Id = 1,
            Username = "old_name",
            Email = "old@test.com",
            RoleId = 1
        };

        var updated = new User
        {
            Username = "new_name",
            Email = "new@test.com",
            RoleId = 2
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existing);

        var result = await _userService.UpdateAsync(1, updated);

        result.Username.Should().Be("new_name");
        result.Email.Should().Be("new@test.com");
        result.RoleId.Should().Be(2);
        _repositoryMock.Verify(x => x.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Update_Should_ThrowNotFoundException_When_UserNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((User?)null);

        var action = async () => await _userService.UpdateAsync(999, new User());

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("User not found");
    }

    [Fact]
    public async Task Delete_Should_RemoveUser_When_Exists()
    {
        var user = new User { Id = 1 };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(user);

        await _userService.DeleteAsync(1);

        _repositoryMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }
}