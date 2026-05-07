using Wardrobe.Data.Entities;

namespace Wardrobe.Services.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}