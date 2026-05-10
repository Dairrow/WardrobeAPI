using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Outfits;

namespace Wardrobe.UnitTests.DTOs.Outfits;

public class CreateOutfitDtoTests
{
	[Fact]
	public void CreateOutfitDto_ValidData_Should_PassValidation()
	{
		var dto = new CreateOutfitDto { Name = "Summer Casual" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void CreateOutfitDto_EmptyName_Should_FailValidation(string invalidName)
	{
		var dto = new CreateOutfitDto { Name = invalidName! };

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
	}

	[Fact]
	public void CreateOutfitDto_NameTooLong_Should_FailValidation()
	{
		var dto = new CreateOutfitDto
		{
			Name = new string('A', 101)
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
	}

	[Fact]
	public void CreateOutfitDto_NameExactly100Chars_Should_PassValidation()
	{
		var dto = new CreateOutfitDto
		{
			Name = new string('A', 100)
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