using FluentAssertions;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;

namespace Wardrobe.UnitTests.Services;

public class OutfitServiceTests
{
    private readonly Mock<IOutfitRepository> _repositoryMock;
    private readonly OutfitService _service;

    public OutfitServiceTests()
    {
        _repositoryMock = new Mock<IOutfitRepository>();
        _service = new OutfitService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByUserId_Should_ReturnUsersOutfits()
    {
        var userId = 1;
        var outfits = new List<Outfit>
        {
            new() { Id = 1, Name = "Summer", UserId = userId },
            new() { Id = 2, Name = "Winter", UserId = userId }
        };

        _repositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(outfits);

        var result = await _service.GetByUserIdAsync(userId);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_Should_ThrowArgumentException_When_UserIdNotSet()
    {
        var outfit = new Outfit { Name = "Test Outfit", UserId = 0 };

        var action = async () => await _service.CreateAsync(outfit);

        await action.Should().ThrowAsync<ArgumentException>()
            .WithMessage("UserId must be set before creating an outfit");
    }

    [Fact]
    public async Task Create_Should_AddOutfit_When_Valid()
    {
        var outfit = new Outfit { Name = "Summer", UserId = 1 };

        _repositoryMock
            .Setup(x => x.AddAsync(outfit))
            .ReturnsAsync((Outfit o) => { o.Id = 1; return o; });

        var result = await _service.CreateAsync(outfit);

        result.Id.Should().Be(1);
        result.Name.Should().Be("Summer");
    }

    [Fact]
    public async Task Update_Should_ThrowNotFoundException_When_OutfitNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Outfit?)null);

        var action = async () =>
            await _service.UpdateAsync(999, new Outfit { Name = "Test" });

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Outfit not found");
    }

    [Fact]
    public async Task Delete_Should_RemoveOutfit_When_Exists()
    {
        var outfit = new Outfit { Id = 1, Name = "Summer" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(outfit);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(x => x.DeleteAsync(outfit), Times.Once);
    }
}