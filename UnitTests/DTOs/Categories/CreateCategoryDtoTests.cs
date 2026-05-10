using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Categories;

namespace Wardrobe.UnitTests.DTOs.Categories;

public class CreateCategoryDtoTests
{
	[Fact]
	public void CreateCategoryDto_ValidData_Should_PassValidation()
	{
		var dto = new CreateCategoryDto { Name = "Shoes" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void CreateCategoryDto_EmptyName_Should_FailValidation(string invalidName)
	{
		var dto = new CreateCategoryDto { Name = invalidName! };

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
		validationResults[0].ErrorMessage.Should().Be("Category name is required");
	}

	[Fact]
	public void CreateCategoryDto_NameTooShort_Should_FailValidation()
	{
		var dto = new CreateCategoryDto { Name = "A" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].ErrorMessage.Should().Contain("between 2 and 100");
	}

	[Fact]
	public void CreateCategoryDto_NameTooLong_Should_FailValidation()
	{
		var dto = new CreateCategoryDto
		{
			Name = new string('B', 101)
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].ErrorMessage.Should().Contain("between 2 and 100");
	}

	[Fact]
	public void CreateCategoryDto_NameWithMinLength_Should_PassValidation()
	{
		var dto = new CreateCategoryDto { Name = "AB" };

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