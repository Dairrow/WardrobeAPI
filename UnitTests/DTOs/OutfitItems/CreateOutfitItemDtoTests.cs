using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.OutfitItems;

namespace Wardrobe.UnitTests.DTOs.OutfitItems;

public class CreateOutfitItemDtoTests
{
    [Fact]
    public void CreateOutfitItemDto_ValidData_Should_PassValidation()
    {
        var dto = new CreateOutfitItemDto
        {
            OutfitId = 1,
            ClothingItemId = 5
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void CreateOutfitItemDto_MissingOutfitId_Should_FailValidation()
    {
        var dto = new CreateOutfitItemDto
        {
            OutfitId = -1,
            ClothingItemId = 5
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("OutfitId");
    }

    [Fact]
    public void CreateOutfitItemDto_MissingClothingItemId_Should_FailValidation()
    {
        var dto = new CreateOutfitItemDto
        {
            OutfitId = 1,
            ClothingItemId = -1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("ClothingItemId");
    }

    [Fact]
    public void CreateOutfitItemDto_BothIdsMissing_Should_FailValidation()
    {
        var dto = new CreateOutfitItemDto
        {
            OutfitId = -1,
            ClothingItemId = -1
        };

        var validationResults = ValidateModel(dto);

        validationResults.Should().HaveCount(2);
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}