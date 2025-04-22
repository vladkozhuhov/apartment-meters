using System;

namespace Application.Models.MeterReadingModel
{
    /// <summary>
    /// DTO для показаний счетчика
    /// </summary>
    public class MeterReadingDto
    {
        /// <summary>
        /// Идентификатор показания
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// Идентификатор счетчика воды
        /// </summary>
        public Guid WaterMeterId { get; set; }
        
        /// <summary>
        /// Значение показания
        /// </summary>
        public string WaterValue { get; set; }
        
        /// <summary>
        /// Разница с предыдущим показанием
        /// </summary>
        public decimal DifferenceValue { get; set; }
        
        /// <summary>
        /// Дата показания
        /// </summary>
        public DateTime ReadingDate { get; set; }
    }
} 