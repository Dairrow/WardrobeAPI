using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Users;

namespace Wardrobe.UnitTests.DTOs.Users;

public class CreateUserDtoTests
{
	[Fact]
	public void CreateUserDto_ValidData_Should_PassValidation()
	{
		var dto = new CreateUserDto
		{
			Username = "john_doe",
			Email = "john@example.com",
			Password = "password123",
			RoleId = 1
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Fact]
	public void CreateUserDto_UsernameTooLong_Should_FailValidation()
	{
		var dto = new CreateUserDto
		{
			Username = new string('A', 101),
			Email = "john@example.com",
			Password = "password123",
			RoleId = 1
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Username");
	}

	[Theory]
	[InlineData("not-an-email")]
	[InlineData("invalid")]
	[InlineData("@nodomain.com")]
	public void CreateUserDto_InvalidEmail_Should_FailValidation(string invalidEmail)
	{
		var dto = new CreateUserDto
		{
			Username = "john_doe",
			Email = invalidEmail,
			Password = "password123",
			RoleId = 1
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Email");
	}

	[Fact]
	public void CreateUserDto_UsernameExactly100Chars_Should_PassValidation()
	{
		var dto = new CreateUserDto
		{
			Username = new string('A', 100),
			Email = "john@example.com",
			Password = "password123",
			RoleId = 1
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