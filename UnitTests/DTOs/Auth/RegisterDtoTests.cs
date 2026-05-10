using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Auth;

namespace Wardrobe.UnitTests.DTOs.Auth;

public class RegisterDtoTests
{
	[Fact]
	public void RegisterDto_ValidData_Should_PassValidation()
	{
		var dto = new RegisterDto
		{
			Username = "john_doe",
			Email = "john@example.com",
			Password = "password123"
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData(null, "john@example.com", "password123", "Username")]
	[InlineData("", "john@example.com", "password123", "Username")]
	[InlineData("john_doe", null, "password123", "Email")]
	[InlineData("john_doe", "", "password123", "Email")]
	[InlineData("john_doe", "john@example.com", null, "Password")]
	[InlineData("john_doe", "john@example.com", "", "Password")]
	public void RegisterDto_MissingRequiredFields_Should_FailValidation(
		string username, string email, string password, string expectedMemberName)
	{
		var dto = new RegisterDto
		{
			Username = username!,
			Email = email!,
			Password = password!
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().NotBeEmpty();
		validationResults.Should().Contain(v => v.MemberNames.Contains(expectedMemberName));
	}

	[Theory]
	[InlineData("not-an-email")]
	[InlineData("invalid")]
	[InlineData("@nodomain")]
	public void RegisterDto_InvalidEmail_Should_FailValidation(string invalidEmail)
	{
		var dto = new RegisterDto
		{
			Username = "john_doe",
			Email = invalidEmail,
			Password = "password123"
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Email");
	}

	[Theory]
	[InlineData("12345")]  // 5 chars - too short
	[InlineData("1234")]   // 4 chars
	[InlineData("123")]	// 3 chars
	public void RegisterDto_PasswordTooShort_Should_FailValidation(string shortPassword)
	{
		var dto = new RegisterDto
		{
			Username = "john_doe",
			Email = "john@example.com",
			Password = shortPassword
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Password");
	}

	[Fact]
	public void RegisterDto_PasswordExactly6Chars_Should_PassValidation()
	{
		var dto = new RegisterDto
		{
			Username = "john_doe",
			Email = "john@example.com",
			Password = "123456"
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