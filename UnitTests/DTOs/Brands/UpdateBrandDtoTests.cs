using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Brands;

namespace Wardrobe.UnitTests.DTOs.Brands;

public class UpdateBrandDtoTests
{
    [Fact]
    public void UpdateBrandDto_ValidData_Should_PassValidation()
    {
        var dto = new UpdateBrandDto { Name = "Adidas Updated" };

        var validationResults = ValidateModel(dto);

        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void UpdateBrandDto_EmptyName_Should_FailValidation()
    {
        var dto = new UpdateBrandDto { Name = null! };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    [Fact]
    public void UpdateBrandDto_NameTooShort_Should_FailValidation()
    {
        var dto = new UpdateBrandDto { Name = "X" };

        var validationResults = ValidateModel(dto);

        validationResults.Should().ContainSingle();
        validationResults[0].MemberNames.Should().Contain("Name");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}