using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Wardrobe.API.DTOs.Users;

namespace Wardrobe.UnitTests.DTOs.Users;

public class UpdateUserDtoTests
{
	[Fact]
	public void UpdateUserDto_ValidData_Should_PassValidation()
	{
		var dto = new UpdateUserDto
		{
			Username = "john_doe_updated",
			Email = "john.updated@example.com",
			RoleId = 2
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().BeEmpty();
	}

	[Theory]
	[InlineData("not-an-email")]
	[InlineData("invalid-email")]
	public void UpdateUserDto_InvalidEmail_Should_FailValidation(string invalidEmail)
	{
		var dto = new UpdateUserDto
		{
			Username = "john_doe",
			Email = invalidEmail,
			RoleId = 1
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Email");
	}

	[Fact]
	public void UpdateUserDto_UsernameTooLong_Should_FailValidation()
	{
		var dto = new UpdateUserDto
		{
			Username = new string('B', 101),
			Email = "john@example.com",
			RoleId = 1
		};

		var validationResults = ValidateModel(dto);

		validationResults.Should().ContainSingle();
		validationResults[0].MemberNames.Should().Contain("Username");
	}

	[Fact]
	public void UpdateUserDto_NoPassword_Should_PassValidation()
	{
		var dto = new UpdateUserDto
		{
			Username = "john_doe",
			Email = "john@example.com",
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