using OMS_Backend.Models;

namespace OMS_Backend.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}
