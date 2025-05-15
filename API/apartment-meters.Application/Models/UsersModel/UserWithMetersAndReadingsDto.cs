using System;
using System.Collections.Generic;
using Application.Models.MeterReadingModel;
using Application.Models.WaterMeterModel;

namespace Application.Models.UsersModel
{
    /// <summary>
    /// DTO для отображения пользователя с его счетчиками и показаниями
    /// </summary>
    public class UserWithMetersAndReadingsDto
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Номер квартиры
        /// </summary>
        public int ApartmentNumber { get; set; }
        
        /// <summary>
        /// Фамилия
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Имя
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Отчество
        /// </summary>
        public string? MiddleName { get; set; }
        
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string PhoneNumber { get; set; }
        
        /// <summary>
        /// Роль пользователя
        /// </summary>
        public int Role { get; set; }
        
        /// <summary>
        /// Счетчики воды пользователя
        /// </summary>
        public List<WaterMeterWithReadingsDto> WaterMeters { get; set; } = new List<WaterMeterWithReadingsDto>();
    }
    
    /// <summary>
    /// DTO для отображения счетчика воды с его показаниями
    /// </summary>
    public class WaterMeterWithReadingsDto
    {
        /// <summary>
        /// Идентификатор счетчика
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }
        
        /// <summary>
        /// Место установки счетчика воды (1 - кухня, 2 - ванная)
        /// </summary>
        public int PlaceOfWaterMeter { get; set; }
        
        /// <summary>
        /// Тип воды (1 - горячая, 2 - холодная)
        /// </summary>
        public int WaterType { get; set; }
        
        /// <summary>
        /// Заводской номер
        /// </summary>
        public string FactoryNumber { get; set; }
        
        /// <summary>
        /// Год выпуска
        /// </summary>
        public DateTime FactoryYear { get; set; }
        
        /// <summary>
        /// Показания счетчика
        /// </summary>
        public List<MeterReadingDto> Readings { get; set; } = new List<MeterReadingDto>();
    }
} 