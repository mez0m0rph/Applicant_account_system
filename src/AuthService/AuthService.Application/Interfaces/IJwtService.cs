// задача сервиса - создавать JWT токен
// принимает пользователя User, подписывает токен секретным ключом
// возвращает строку JWT
// User -> JwtService -> Token (string)

using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}

// сервис, который User -> JWT string