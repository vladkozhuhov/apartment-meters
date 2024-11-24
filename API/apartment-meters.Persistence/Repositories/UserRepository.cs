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
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Конструктор репозитория
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Пользователь или null, если не найден</returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Получить всех пользователей
    /// </summary>
    /// <returns>Список всех пользователей</returns>
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    /// <summary>
    /// Добавить нового пользователя
    /// </summary>
    /// <param name="user">Пользователь для добавления</param>
    /// <returns>Задача, которая завершится после добавления</returns>
    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Обновить информацию о пользователе
    /// </summary>
    /// <param name="user">Информация о пользователе для обновления</param>
    /// <returns>Задача, которая завершится после обновления</returns>
    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="user">Пользователь для удаления</param>
    /// <returns>Задача, которая завершится после удаления</returns>
    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Получить пользователя по номеру квартиры
    /// </summary>
    /// <param name="apartmentNumber">Номер квартиры</param>
    /// <returns>Получение пользователя по номеру квартиры</returns>
    public async Task<User?> GetByApartmentNumberAsync(int apartmentNumber)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.ApartmentNumber == apartmentNumber);
    }
}