using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wardrobe.Data.Entities;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class JwtService : IJwtService
{
	private readonly IConfiguration _configuration;


	public JwtService(
		IConfiguration configuration)
	{
		_configuration = configuration;
	}


	public string GenerateToken(
		User user)
	{
		var key =
			new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(
					_configuration["JwtSettings:SecretKey"]!));


		var credentials =
			new SigningCredentials(
				key,
				SecurityAlgorithms.HmacSha256);


		var claims =
			new List<Claim>
			{
				new(ClaimTypes.NameIdentifier,
					user.Id.ToString()),

				new(ClaimTypes.Email,
					user.Email),

				new(ClaimTypes.Role,
					user.Role.Name)
			};


		var token =
			new JwtSecurityToken(
				issuer:
					_configuration[
						"JwtSettings:Issuer"],

				audience:
					_configuration[
						"JwtSettings:Audience"],

				claims:
					claims,

				expires:
					DateTime.UtcNow.AddMinutes(
						int.Parse(
							_configuration[
								"JwtSettings:ExpirationInMinutes"]!)),

				signingCredentials:
					credentials);


		return new JwtSecurityTokenHandler()
			.WriteToken(token);
	}
}