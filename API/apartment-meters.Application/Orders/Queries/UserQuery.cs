using Application.Interfaces.Queries;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Models.UsersModel;
using Application.Models.MeterReadingModel;

namespace Application.Orders.Queries;

/// <summary>
/// Сервис для выполнения операций чтения пользователей
/// </summary>
public class UserQuery : IUserQuery
{
    private readonly IUserRepository _userRepository;
    private readonly ICachedRepository<UserEntity, Guid> _cachedRepository;
    private readonly ILogger<UserQuery> _logger;
    private readonly IWaterMeterRepository _waterMeterRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="userRepository">Репозиторий для работы с сущностью User</param>
    /// <param name="cachedRepository">Кэширующий репозиторий пользователей</param>
    /// <param name="logger">Сервис логирования</param>
    /// <param name="waterMeterRepository">Репозиторий для работы со счетчиками</param>
    /// <param name="meterReadingRepository">Репозиторий для работы с показаниями</param>
    public UserQuery(
        IUserRepository userRepository,
        ICachedRepository<UserEntity, Guid> cachedRepository,
        IWaterMeterRepository waterMeterRepository,
        IMeterReadingRepository meterReadingRepository,
        ILogger<UserQuery> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _cachedRepository = cachedRepository ?? throw new ArgumentNullException(nameof(cachedRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _waterMeterRepository = waterMeterRepository ?? throw new ArgumentNullException(nameof(waterMeterRepository));
        _meterReadingRepository = meterReadingRepository ?? throw new ArgumentNullException(nameof(meterReadingRepository));
    }
    
    /// <inheritdoc />
    public async Task<UserEntity> GetUserByIdAsync(Guid id)
    {
        _logger.LogInformation("Получение пользователя с ID {UserId}", id);
        var user = await _cachedRepository.GetByIdCachedAsync(id) ??
                  await _userRepository.GetByIdAsync(id);
                  
        if (user == null)
        {
            throw new KeyNotFoundException($"Пользователь с ID {id} не найден");
        }
        
        return user;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserEntity>> GetAllUsersAsync()
    {
        _logger.LogInformation("Получение всех пользователей");
        return await _cachedRepository.GetAllCachedAsync();
    }
    
    /// <inheritdoc />
    public async Task<UserEntity> GetUserByApartmentNumberAsync(int apartmentNumber)
    {
        _logger.LogInformation("Получение пользователя с номером квартиры {ApartmentNumber}", apartmentNumber);
        
        // Проверяем в кэше
        var allUsers = await _cachedRepository.GetAllCachedAsync();
        var cachedUser = allUsers.FirstOrDefault(u => u.ApartmentNumber == apartmentNumber);
        
        if (cachedUser != null)
        {
            return cachedUser;
        }
        
        // Если не найдено в кэше, запрашиваем из репозитория
        var user = await _userRepository.GetByApartmentNumberAsync(apartmentNumber);
        
        if (user == null)
        {
            throw new KeyNotFoundException($"Пользователь с номером квартиры {apartmentNumber} не найден");
        }
        
        return user;
    }

    /// <inheritdoc />
    public async Task<int> GetAllUsersCountAsync()
    {
        _logger.LogInformation("Получение общего количества пользователей");
        var users = await _cachedRepository.GetAllCachedAsync();
        return users.Count();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<UserWithMetersAndReadingsDto>> GetUsersWithMetersAndReadingsAsync(int page, int pageSize)
    {
        _logger.LogInformation("Получение пользователей с их счетчиками и показаниями (страница {Page}, размер {PageSize})", page, pageSize);
        
        // Получаем всех пользователей (потом применим пагинацию)
        var users = await _cachedRepository.GetAllCachedAsync();
        
        // Применяем пагинацию
        var paginatedUsers = users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        // Создаем список результатов
        var result = new List<UserWithMetersAndReadingsDto>();
        
        // Для каждого пользователя получаем его счетчики и показания
        foreach (var user in paginatedUsers)
        {
            var userDto = new UserWithMetersAndReadingsDto
            {
                Id = user.Id,
                ApartmentNumber = user.ApartmentNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                PhoneNumber = user.PhoneNumber,
                Role = (int)user.Role,
                WaterMeters = new List<WaterMeterWithReadingsDto>()
            };
            
            // Получаем счетчики пользователя
            var waterMeters = await _waterMeterRepository.GetByUserIdAsync(user.Id);
            
            // Для каждого счетчика получаем его показания
            foreach (var waterMeter in waterMeters)
            {
                var waterMeterDto = new WaterMeterWithReadingsDto
                {
                    Id = waterMeter.Id,
                    UserId = waterMeter.UserId,
                    PlaceOfWaterMeter = (int)waterMeter.PlaceOfWaterMeter,
                    WaterType = (int)waterMeter.WaterType,
                    FactoryNumber = waterMeter.FactoryNumber,
                    FactoryYear = DateTime.Parse(waterMeter.FactoryYear.ToString()),
                    Readings = new List<MeterReadingDto>()
                };
                
                // Получаем показания счетчика
                var readings = await _meterReadingRepository.GetByWaterMeterIdAsync(waterMeter.Id);
                
                // Преобразуем показания в DTO и добавляем в счетчик
                foreach (var reading in readings)
                {
                    waterMeterDto.Readings.Add(new MeterReadingDto
                    {
                        Id = reading.Id,
                        WaterMeterId = reading.WaterMeterId,
                        WaterValue = reading.WaterValue,
                        DifferenceValue = (decimal)reading.DifferenceValue,
                        ReadingDate = reading.ReadingDate
                    });
                }
                
                // Добавляем счетчик в пользователя
                userDto.WaterMeters.Add(waterMeterDto);
            }
            
            // Добавляем пользователя в результат
            result.Add(userDto);
        }
        
        return result;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<UserEntity>> GetPaginatedUsersAsync(int page, int pageSize)
    {
        _logger.LogInformation("Получение пагинированных пользователей (страница {Page}, размер {PageSize})", page, pageSize);
        
        // Получаем всех пользователей из кэша
        var users = await _cachedRepository.GetAllCachedAsync();
        
        // Применяем пагинацию
        var paginatedUsers = users
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        return paginatedUsers;
    }
}