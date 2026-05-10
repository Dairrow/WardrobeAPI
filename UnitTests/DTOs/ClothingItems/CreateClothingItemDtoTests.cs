using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.ClothingItems;

namespace Wardrobe.UnitTests.DTOs.ClothingItems;

public class CreateClothingItemDtoTests
{
    [Fact]
    public void CreateClothingItemDto_ValidData_Should_PassValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = "Classic T-Shirt",
            Price = 29.99m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CreateClothingItemDto_EmptyName_Should_FailValidation(string invalidName)
    {
        var dto = new CreateClothingItemDto
        {
            Name = invalidName!,
            Price = 29.99m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    [Fact]
    public void CreateClothingItemDto_NameTooLong_Should_FailValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = new string('A', 101),
            Price = 29.99m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(0.001)]
    public void CreateClothingItemDto_InvalidPrice_Should_FailValidation(decimal invalidPrice)
    {
        var dto = new CreateClothingItemDto
        {
            Name = "T-Shirt",
            Price = invalidPrice,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Price");
    }

    [Fact]
    public void CreateClothingItemDto_PriceTooHigh_Should_FailValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = "Luxury Item",
            Price = 100000.01m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Price");
    }

    [Fact]
    public void CreateClothingItemDto_MinPrice_Should_PassValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = "Cheap Item",
            Price = 0.01m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateClothingItemDto_MaxPrice_Should_PassValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = "Expensive Item",
            Price = 100000m,
            CategoryId = 1,
            BrandId = 1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateClothingItemDto_WithoutImage_Should_PassValidation()
    {
        var dto = new CreateClothingItemDto
        {
            Name = "T-Shirt",
            Price = 29.99m,
            CategoryId = 1,
            BrandId = 1,
            Image = null
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}