using Application.Models.UsersModel;
using Domain.Entities;

namespace Application.Interfaces.Commands;

/// <summary>
/// Интерфейс для управления командами, связанными с пользователями
/// </summary>
public interface IUserCommand
{
    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="dto">DTO с данными нового пользователя</param>
    /// <returns>Идентификатор добавленного пользователя</returns>
    Task<UserEntity> AddUserAsync(UserAddDto dto);
    
    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="dto">DTO с обновленными данными пользователя</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateUserAsync(Guid userId, UserUpdateDto dto);

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя для удаления</param>
    /// <returns>Task для отслеживания операции</returns>
    Task DeleteUserAsync(Guid id);
    
    /// <summary>
    /// Обновить номер телефона пользователя
    /// </summary>
    /// <param name="userId">Идентификатор пользователя</param>
    /// <param name="phoneDto">DTO с новым номером телефона</param>
    /// <returns>Task для отслеживания операции</returns>
    Task UpdateUserPhoneAsync(Guid userId, PhoneUpdateDto phoneDto);
}