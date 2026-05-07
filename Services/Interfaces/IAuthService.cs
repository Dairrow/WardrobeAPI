using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IAuthService
{
    Task<User> RegisterAsync(
        string username,
        string email,
        string password);


    Task<string> LoginAsync(
        string email,
        string password);
}