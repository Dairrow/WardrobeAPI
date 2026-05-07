using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Wardrobe.API.Extensions;

public static class JwtExtensions
{
    public static IServiceCollection AddJwt(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var key =
            Encoding.UTF8.GetBytes(
                configuration[
                    "JwtSettings:SecretKey"]!);


        services.AddAuthentication(
                JwtBearerDefaults
                    .AuthenticationScheme)
            .AddJwtBearer(
                options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                            ValidateIssuer = true,

                            ValidateAudience = true,

                            ValidateLifetime = true,

                            ValidateIssuerSigningKey = true,

                            ValidIssuer =
                                configuration[
                                    "JwtSettings:Issuer"],

                            ValidAudience =
                                configuration[
                                    "JwtSettings:Audience"],

                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    key)
                        };
                });


        return services;
    }
}