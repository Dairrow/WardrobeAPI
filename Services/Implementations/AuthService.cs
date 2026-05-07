using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Helpers;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class AuthService
    : IAuthService
{
    private readonly IUserRepository _users;

    private readonly IRoleRepository _roles;

    private readonly IJwtService _jwt;


    public AuthService(
        IUserRepository users,
        IRoleRepository roles,
        IJwtService jwt)
    {
        _users = users;

        _roles = roles;

        _jwt = jwt;
    }


    public async Task<User> RegisterAsync(
        string username,
        string email,
        string password)
    {
        var existing =
            await _users.GetByEmailAsync(
                email);


        if (existing is not null)
        {
            throw new Exception(
                "User already exists");
        }


        var role =
            await _roles.GetByNameAsync(
                "Admin");


        if (role is null)
        {
            throw new Exception(
                "Admin role not found");
        }


        var user =
            new User
            {
                Username = username,

                Email = email,

                PasswordHash =
                    PasswordHasher.Hash(
                        password),

                RoleId = role.Id,

                Role = role
            };


        return await _users.AddAsync(
            user);
    }


    public async Task<string> LoginAsync(
        string email,
        string password)
    {
        var user =
            await _users.GetByEmailAsync(
                email);


        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }


        var hash =
            PasswordHasher.Hash(
                password);


        if (user.PasswordHash != hash)
        {
            throw new UnauthorizedAccessException();
        }


        return _jwt.GenerateToken(
            user);
    }
}