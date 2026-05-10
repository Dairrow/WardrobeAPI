using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Brands;

namespace Wardrobe.UnitTests.DTOs.Brands;

public class CreateBrandDtoTests
{
	[Fact]
	public void CreateBrandDto_ValidData_Should_PassValidation()
	{
		var dto = new CreateBrandDto { Name = "Nike" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	public void CreateBrandDto_EmptyName_Should_FailValidation(string invalidName)
	{
		var dto = new CreateBrandDto { Name = invalidName! };

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
	}

	[Fact]
	public void CreateBrandDto_NameTooShort_Should_FailValidation()
	{
		var dto = new CreateBrandDto { Name = "A" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
	}

	[Fact]
	public void CreateBrandDto_NameTooLong_Should_FailValidation()
	{
		var dto = new CreateBrandDto
		{
			Name = new string('A', 101)
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Name");
	}

	[Fact]
	public void CreateBrandDto_NameExactly2Chars_Should_PassValidation()
	{
		var dto = new CreateBrandDto { Name = "AB" };

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Fact]
	public void CreateBrandDto_NameExactly100Chars_Should_PassValidation()
	{
		var dto = new CreateBrandDto
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