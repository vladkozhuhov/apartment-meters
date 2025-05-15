using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.UsersModel;
using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Интерфейс для выполнения операций чтения пользователей
/// </summary>
public interface IUserQuery
{
    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Сущность пользователя</returns>
    Task<UserEntity> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список пользователей</returns>
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();

    /// <summary>
    /// Получить пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Сущность пользователя</returns>
    Task<UserEntity> GetUserByApartmentNumberAsync(int apartmentNumber);

    /// <summary>
    /// Получить общее количество пользователей
    /// </summary>
    /// <returns>Число пользователей</returns>
    Task<int> GetAllUsersCountAsync();

    /// <summary>
    /// Получить пользователей с их счетчиками и показаниями
    /// </summary>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <returns>Список пользователей с их счетчиками и показаниями</returns>
    Task<IEnumerable<UserWithMetersAndReadingsDto>> GetUsersWithMetersAndReadingsAsync(int page, int pageSize);
    
    /// <summary>
    /// Получить пагинированный список пользователей
    /// </summary>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <returns>Список пользователей для текущей страницы</returns>
    Task<IEnumerable<UserEntity>> GetPaginatedUsersAsync(int page, int pageSize);
}