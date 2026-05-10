using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Categories;

namespace Wardrobe.UnitTests.DTOs.Categories;

public class UpdateCategoryDtoTests
{
    [Fact]
    public void UpdateCategoryDto_ValidData_Should_PassValidation()
    {
        var dto = new UpdateCategoryDto { Name = "T-Shirts Updated" };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateCategoryDto_EmptyName_Should_FailValidation()
    {
        var dto = new UpdateCategoryDto { Name = null! };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    [Fact]
    public void UpdateCategoryDto_NameTooShort_Should_FailValidation()
    {
        var dto = new UpdateCategoryDto { Name = "X" };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}