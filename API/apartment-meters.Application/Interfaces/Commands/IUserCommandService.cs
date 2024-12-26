using Application.Models;
using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для управления командами, связанными с пользователями
/// </summary>
public interface IUserCommandService
{
    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="dto">DTO с данными нового пользователя</param>
    /// <returns>Идентификатор добавленного пользователя</returns>
    Task<User> AddUserAsync(AddUserDto dto);

    /// <summary>
    /// Получить данные пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<User> GetUserByIdAsync(Guid id);
    
    /// <summary>
    /// Обновить данные пользователя.
    /// </summary>
    /// <param name="dto">DTO с обновленными данными пользователя</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateUserAsync(UpdateUserDto dto);

    /// <summary>
    /// Удалить пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя для удаления</param>
    /// <returns>Task для отслеживания операции</returns>
    Task DeleteUserAsync(Guid id);
}