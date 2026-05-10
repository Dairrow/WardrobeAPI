using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace IntegrationTests.Infrastructure;

public static class JwtTokenGenerator
{
	private const string SecretKey = "WARDROBE_SUPER_SECRET_KEY_2026_256_BITS";
	private const string Issuer = "Wardrobe.API";
	private const string Audience = "Wardrobe.Client";
	private const int ExpirationInMinutes = 60;

	public static string GenerateAdminToken(int userId = 1, string email = "admin@wardrobe.local")
	{
		return GenerateToken(userId, email, "Admin");
	}

	public static string GenerateUserToken(int userId = 2, string email = "user@wardrobe.local")
	{
		return GenerateToken(userId, email, "User");
	}

	public static string GenerateToken(int userId, string email, string role)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, userId.ToString()),
			new(ClaimTypes.Email, email),
			new(ClaimTypes.Role, role)
		};

		var token = new JwtSecurityToken(
			issuer: Issuer,
			audience: Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(ExpirationInMinutes),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public static string GenerateExpiredToken(int userId = 1, string email = "admin@wardrobe.local", string role = "Admin")
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));

		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, userId.ToString()),
			new(ClaimTypes.Email, email),
			new(ClaimTypes.Role, role)
		};

		var token = new JwtSecurityToken(
			issuer: Issuer,
			audience: Audience,
			claims: claims,
			expires: DateTime.UtcNow.AddMinutes(-1),
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}