using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.ClothingItems;

namespace Wardrobe.UnitTests.DTOs.ClothingItems;

public class UpdateClothingItemDtoTests
{
    [Fact]
    public void UpdateClothingItemDto_ValidData_Should_PassValidation()
    {
        var dto = new UpdateClothingItemDto
        {
            Name = "Updated T-Shirt",
            Price = 39.99m,
            CategoryId = 2,
            BrandId = 2
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateClothingItemDto_EmptyName_Should_FailValidation()
    {
        var dto = new UpdateClothingItemDto
        {
            Name = null!,
            Price = 39.99m
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    [Fact]
    public void UpdateClothingItemDto_InvalidPrice_Should_FailValidation()
    {
        var dto = new UpdateClothingItemDto
        {
            Name = "T-Shirt",
            Price = -5m
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Price");
    }

    [Fact]
    public void UpdateClothingItemDto_WithoutCategoryId_Should_PassValidation()
    {
        var dto = new UpdateClothingItemDto
        {
            Name = "T-Shirt",
            Price = 39.99m,
            BrandId = 1
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