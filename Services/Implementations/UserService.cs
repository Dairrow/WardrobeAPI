using Microsoft.Extensions.Logging;
using Wardrobe.Data.Entities;
using Wardrobe.Repositories.Interfaces;
using Wardrobe.Services.Interfaces;

namespace Wardrobe.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    private readonly ILogger<UserService> _logger;


    public UserService(
        IUserRepository repository,
        ILogger<UserService> logger)
    {
        _repository = repository;

        _logger = logger;
    }


    public async Task<User?> GetByIdAsync(int id)
    {
        _logger.LogInformation(
            "Getting user by id {UserId}",
            id);

        return await _repository.GetByIdAsync(id);
    }


    public async Task<User?> GetByEmailAsync(string email)
    {
        _logger.LogInformation(
            "Getting user by email {Email}",
            email);

        return await _repository.GetByEmailAsync(email);
    }


    public async Task<IEnumerable<User>> GetAllAsync()
    {
        _logger.LogInformation(
            "Getting all users");

        return await _repository.GetAllAsync();
    }


    public async Task<User> CreateAsync(User user)
    {
        var existingUser =
            await _repository.GetByEmailAsync(
                user.Email);


        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "User already exists");
        }


        _logger.LogInformation(
            "Creating user {Email}",
            user.Email);


        return await _repository.AddAsync(
            user);
    }
}