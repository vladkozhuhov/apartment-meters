using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.UsersModel;
using Domain.Entities;

namespace Application.Interfaces.Queries;

/// <summary>
/// Сервис для выполнения запросов, связанных с пользователями
/// </summary>
public interface IUserQuery
{
    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Коллекция пользователей</returns>
    Task<IEnumerable<UserEntity>> GetAllUsersAsync();
    
    /// <summary>
    /// Получить данные пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<UserEntity> GetUserByIdAsync(Guid id);
    
    /// <summary>
    /// Получить данные пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Задача, содержащая данные пользователя или null, если пользователь не найден</returns>
    Task<UserEntity> GetUserByApartmentNumberAsync(int apartmentNumber);
    
    /// <summary>
    /// Получить общее количество пользователей
    /// </summary>
    Task<int> GetAllUsersCountAsync();
    
    /// <summary>
    /// Получить пользователей с их счетчиками и показаниями с пагинацией
    /// </summary>
    /// <param name="page">Номер страницы (начиная с 1)</param>
    /// <param name="pageSize">Размер страницы</param>
    Task<IEnumerable<UserWithMetersAndReadingsDto>> GetUsersWithMetersAndReadingsAsync(int page, int pageSize);
}