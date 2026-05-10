using FluentAssertions;
using Moq;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Exceptions;
using Wardrobe.Services.Implementations;

namespace Wardrobe.UnitTests.Services;

public class ClothingItemServiceTests
{
    private readonly Mock<IClothingItemRepository> _repositoryMock;
    private readonly ClothingItemService _service;

    public ClothingItemServiceTests()
    {
        _repositoryMock = new Mock<IClothingItemRepository>();
        _service = new ClothingItemService(_repositoryMock.Object);
    }

    [Fact]
    public async Task GetByUserId_Should_ReturnUsersItems()
    {
        var userId = 1;
        var items = new List<ClothingItem>
        {
            new() { Id = 1, Name = "T-Shirt", UserId = userId },
            new() { Id = 2, Name = "Jeans", UserId = userId }
        };

        _repositoryMock
            .Setup(x => x.GetByUserIdAsync(userId))
            .ReturnsAsync(items);

        var result = await _service.GetByUserIdAsync(userId);

        result.Should().HaveCount(2);
        result.All(i => i.UserId == userId).Should().BeTrue();
    }

    [Fact]
    public async Task GetById_Should_ReturnItemWithDetails()
    {
        var item = new ClothingItem
        {
            Id = 1,
            Name = "T-Shirt",
            UserId = 1,
            Category = new Category { Name = "T-Shirts" },
            Brand = new Brand { Name = "Nike" }
        };

        _repositoryMock
            .Setup(x => x.GetByIdWithDetailsAsync(1, 1))
            .ReturnsAsync(item);

        var result = await _service.GetByIdAsync(1, 1);

        result.Should().NotBeNull();
        result!.Category.Should().NotBeNull();
        result.Brand.Should().NotBeNull();
    }

    [Fact]
    public async Task Create_Should_AddItem()
    {
        var item = new ClothingItem
        {
            Name = "New Shirt",
            Price = 29.99m,
            CategoryId = 1,
            BrandId = 1,
            UserId = 1
        };

        _repositoryMock
            .Setup(x => x.AddAsync(item))
            .ReturnsAsync((ClothingItem i) => { i.Id = 1; return i; });

        var result = await _service.CreateAsync(item);

        result.Id.Should().Be(1);
        result.Name.Should().Be("New Shirt");
    }

    [Fact]
    public async Task Update_Should_UpdateAllFields()
    {
        var existing = new ClothingItem
        {
            Id = 1,
            Name = "Old",
            Price = 10m,
            CategoryId = 1,
            BrandId = 1,
            ImagePath = "old.jpg"
        };

        var updated = new ClothingItem
        {
            Name = "New",
            Price = 20m,
            CategoryId = 2,
            BrandId = 2,
            ImagePath = "new.jpg"
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existing);

        var result = await _service.UpdateAsync(1, updated);

        result.Name.Should().Be("New");
        result.Price.Should().Be(20m);
        result.CategoryId.Should().Be(2);
        result.BrandId.Should().Be(2);
        result.ImagePath.Should().Be("new.jpg");
        _repositoryMock.Verify(x => x.UpdateAsync(existing), Times.Once);
    }

    [Fact]
    public async Task Update_Should_KeepOldImage_When_NewImageIsNull()
    {
        var existing = new ClothingItem { Id = 1, ImagePath = "old.jpg" };
        var updated = new ClothingItem { Name = "New", ImagePath = null! };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existing);

        var result = await _service.UpdateAsync(1, updated);

        result.ImagePath.Should().Be("old.jpg");
    }

    [Fact]
    public async Task Update_Should_ThrowNotFoundException_When_ItemNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((ClothingItem?)null);

        var action = async () =>
            await _service.UpdateAsync(999, new ClothingItem());

        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage("Clothing item not found");
    }

    [Fact]
    public async Task Delete_Should_RemoveItem_When_Exists()
    {
        var item = new ClothingItem { Id = 1 };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(item);

        await _service.DeleteAsync(1);

        _repositoryMock.Verify(x => x.DeleteAsync(item), Times.Once);
    }

    [Fact]
    public async Task Delete_Should_ThrowNotFoundException_When_ItemNotExists()
    {
        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((ClothingItem?)null);

        var action = async () => await _service.DeleteAsync(999);

        await action.Should().ThrowAsync<NotFoundException>();
    }
}