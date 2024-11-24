using Application.Models;

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
    Task<Guid> AddUserAsync(AddUserDto dto);

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