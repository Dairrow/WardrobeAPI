using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;

namespace Wardrobe.UnitTests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repositoryMock;
    private readonly Mock<ILogger<CategoryService>> _loggerMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _repositoryMock = new Mock<ICategoryRepository>();
        _loggerMock = new Mock<ILogger<CategoryService>>();
        _categoryService = new CategoryService(_repositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAll_Should_LogAndReturnAllCategories()
    {
        var categories = new List<Category>
        {
            new() { Id = 1, Name = "Shoes" },
            new() { Id = 2, Name = "T-Shirts" }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(categories);

        var result = await _categoryService.GetAllAsync();

        result.Should().HaveCount(2);
        _loggerMock.VerifyLog(LogLevel.Information, "Getting categories");
    }

    [Fact]
    public async Task GetById_Should_ReturnCategory_When_Exists()
    {
        var category = new Category { Id = 1, Name = "Shoes" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);

        var result = await _categoryService.GetByIdAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Shoes");
    }

    [Fact]
    public async Task Create_Should_LogAndAddCategory()
    {
        var category = new Category { Name = "Accessories" };

        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category c) => { c.Id = 3; return c; });

        var result = await _categoryService.CreateAsync(category);

        result.Id.Should().Be(3);
        result.Name.Should().Be("Accessories");
        _loggerMock.VerifyLog(LogLevel.Information, "Creating category Accessories");
    }

    [Fact]
    public async Task Update_Should_ThrowNotFoundException_When_CategoryNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        var action = async () =>
            await _categoryService.UpdateAsync(999, new Category { Name = "Test" });

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Category not found");
    }

    [Fact]
    public async Task Update_Should_UpdateName_When_CategoryExists()
    {
        var existingCategory = new Category { Id = 1, Name = "Old" };
        var updatedCategory = new Category { Name = "New" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCategory);

        var result = await _categoryService.UpdateAsync(1, updatedCategory);

        result.Name.Should().Be("New");
        _repositoryMock.Verify(x => x.UpdateAsync(existingCategory), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_RemoveCategory_When_Exists()
    {
        var category = new Category { Id = 1, Name = "Shoes" };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(category);

        await _categoryService.DeleteAsync(1);

        _repositoryMock.Verify(x => x.DeleteAsync(category), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_ThrowNotFoundException_When_CategoryNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Category?)null);

        var action = async () => await _categoryService.DeleteAsync(999);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}

public static class LoggerExtensions
{
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> loggerMock,
        LogLevel level,
        string contains,
        Times? times = null)
    {
        loggerMock.Verify(
            x => x.Log(
                level,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains(contains)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            times ?? Times.Once());
    }
}