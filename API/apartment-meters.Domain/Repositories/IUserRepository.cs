using Domain.Entities;

namespace Domain.Repositories;

public interface IUserRepository
{
    Task<User> GetUserByIdAsync(Guid id); // Получение пользователя по Id
    Task<List<User>> GetAllUsersAsync(); // Получение всех пользователей
    Task AddUserAsync(User user);        // Добавление нового пользователя
    Task UpdateUserAsync(User user);     // Обновление существующего пользователя
    Task DeleteUserAsync(Guid id);       // Удаление пользователя
}