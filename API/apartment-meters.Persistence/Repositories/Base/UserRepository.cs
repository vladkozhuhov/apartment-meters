using Domain.Entities;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence.Repositories;

/// <summary>
/// Реализация репозитория для работы с пользователями
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="dbContext">Контекст базы данных</param>
    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    public async Task<UserEntity?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список всех пользователей</returns>
    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .ToListAsync();
    }

    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="userEntity">Пользователь для добавления</param>
    /// <returns>Задача, которая завершится после добавления</returns>
    public async Task<UserEntity> AddAsync(UserEntity userEntity)
    {
        await _dbContext.Users.AddAsync(userEntity);
        await _dbContext.SaveChangesAsync();

        return userEntity;
    }

    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    /// <param name="userEntity">Информация о пользователе для обновления</param>
    /// <returns>Задача, которая завершится после обновления</returns>
    public async Task UpdateAsync(UserEntity userEntity)
    {
        _dbContext.Users.Update(userEntity);
        await _dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="userEntity">Пользователь для удаления</param>
    /// <returns>Задача, которая завершится после удаления</returns>
    public async Task DeleteAsync(UserEntity userEntity)
    {
        _dbContext.Users.Remove(userEntity);
        await _dbContext.SaveChangesAsync();
    }
    
    /// <summary>
    /// Получить пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Получение пользователя по номеру квартиры</returns>
    public async Task<UserEntity?> GetByApartmentNumberAsync(int apartmentNumber)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.ApartmentNumber == apartmentNumber);
    }
}