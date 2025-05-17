using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Models.UsersModel
{
    /// <summary>
    /// DTO для обновления номера телефона пользователя
    /// </summary>
    public class PhoneUpdateDto
    {
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        [Required(ErrorMessage = "Номер телефона обязателен")]
        [Phone(ErrorMessage = "Неверный формат номера телефона")]
        [JsonPropertyName("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        // Добавляем это свойство для случая, когда с фронтенда приходит phoneNumber в нижнем регистре
        [JsonIgnore]
        public string phoneNumber 
        { 
            get => PhoneNumber; 
            set => PhoneNumber = value; 
        }
    }
} 