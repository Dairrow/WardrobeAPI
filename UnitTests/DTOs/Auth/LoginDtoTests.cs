using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Auth;

namespace Wardrobe.UnitTests.DTOs.Auth;

public class LoginDtoTests
{
	[Fact]
	public void LoginDto_ValidData_Should_PassValidation()
	{
		var dto = new LoginDto
		{
			Email = "admin@wardrobe.local",
			Password = "Admin123!"
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData(null, "password123", "Email")]
	[InlineData("", "password123", "Email")]
	[InlineData("user@example.com", null, "Password")]
	[InlineData("user@example.com", "", "Password")]
	public void LoginDto_MissingRequiredFields_Should_FailValidation(
		string email, string password, string expectedMemberName)
	{
		var dto = new LoginDto
		{
			Email = email!,
			Password = password!
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain(expectedMemberName);
		validationResults[0].ErrorMessage.Should().Contain("required");
	}

	private static List<ValidationResult> ValidateModel(object model)
	{
		var validationResults = new List<ValidationResult>();
		var validationContext = new ValidationContext(model);
		Validator.TryValidateObject(model, validationContext, validationResults, true);
		return validationResults;
	}
}