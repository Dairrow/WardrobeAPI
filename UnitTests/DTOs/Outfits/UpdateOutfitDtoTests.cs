using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Outfits;

namespace Wardrobe.UnitTests.DTOs.Outfits;

public class UpdateOutfitDtoTests
{
	[Fact]
	public void UpdateOutfitDto_ValidData_Should_PassValidation()
	{
		var dto = new UpdateOutfitDto { Name = "Winter Collection" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Fact]
	public void UpdateOutfitDto_EmptyName_Should_FailValidation()
	{
		var dto = new UpdateOutfitDto { Name = null! };

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