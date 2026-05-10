using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;

namespace Wardrobe.UnitTests.Services;

public class BrandServiceTests
{
    private readonly Mock<IBrandRepository> _repositoryMock;
    private readonly Mock<ILogger<BrandService>> _loggerMock;
    private readonly BrandService _brandService;

    public BrandServiceTests()
    {
        _repositoryMock = new Mock<IBrandRepository>();
        _loggerMock = new Mock<ILogger<BrandService>>();
        _brandService = new BrandService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_Should_ReturnAllBrands()
    {
        var brands = new List<Brand>
        {
            new() { Id = 1, Name = "Nike" },
            new() { Id = 2, Name = "Adidas" }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(brands);

        var result = await _brandService.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().Contain(b => b.Name == "Nike");
    }

    [Fact]
    public async Task GetById_Should_ReturnBrand_When_Exists()
    {
        var brand = new Brand { Id = 1, Name = "Nike" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(brand);

        var result = await _brandService.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Nike");
    }

    [Fact]
    public async Task GetById_Should_ReturnNull_When_NotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Brand?)null);

        var result = await _brandService.GetByIdAsync(999);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_Should_AddBrand_And_ReturnIt()
    {
        var brand = new Brand { Name = "Puma" };

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Brand>()))
            .ReturnsAsync((Brand b) => { b.Id = 1; return b; });

        var result = await _brandService.CreateAsync(brand);

        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Puma");
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_UpdateBrand_When_Exists()
    {
        var existingBrand = new Brand { Id = 1, Name = "Old Name" };
        var updatedBrand = new Brand { Name = "New Name" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingBrand);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Brand>()))
            .Returns(Task.CompletedTask);

        var result = await _brandService.UpdateAsync(1, updatedBrand);

        result.Name.Should().Be("New Name");
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Once);
    }

    [Fact]
    public async Task Update_Should_ThrowNotFoundException_When_BrandNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Brand?)null);

        var action = async () =>
            await _brandService.UpdateAsync(999, new Brand { Name = "Test" });

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Brand not found");
    }

    [Fact]
    public async Task Delete_Should_RemoveBrand_When_Exists()
    {
        var brand = new Brand { Id = 1, Name = "Nike" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(brand);

        _repositoryMock
            .Setup(x => x.DeleteAsync(brand))
            .Returns(Task.CompletedTask);

        await _brandService.DeleteAsync(1);

        _repositoryMock.Verify(x => x.DeleteAsync(brand), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_ThrowNotFoundException_When_BrandNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Brand?)null);

        var action = async () => await _brandService.DeleteAsync(999);

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Brand not found");
    }
}