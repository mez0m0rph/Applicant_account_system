// задача сервиса - создавать JWT токен
// принимает пользователя User, подписывает токен секретным ключом
// возвращает строку JWT
// User -> JwtService -> Token (string)

using AuthService.Models;

namespace AuthService.Services;

public interface IJwtService
{
    string GenerateToken(User user);
}

// сервис, который User -> JWT string