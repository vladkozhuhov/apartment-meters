using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Converters
{
    /// <summary>
    /// Конвертер для преобразования типа DateOnly в JSON и обратно
    /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateOnly>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string? dateStr = reader.GetString();
                if (string.IsNullOrEmpty(dateStr))
                {
                    throw new JsonException("Дата не может быть пустой");
                }
                
                return DateOnly.Parse(dateStr);
            }
            
            throw new JsonException("Неверный формат даты. Ожидается строка в формате yyyy-MM-dd");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat));
        }
    }
} 