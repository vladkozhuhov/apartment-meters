/**
 * Конфигурация API для приложения
 */

// Базовый URL для API
export const API_BASE_URL = '';

// Таймаут для запросов (в миллисекундах)
export const API_TIMEOUT = 15000; // 15 секунд

// Максимальное количество повторных попыток при неудачном запросе
export const API_MAX_RETRIES = 1;

// Заголовки по умолчанию для запросов
export const API_DEFAULT_HEADERS = {
  'Content-Type': 'application/json',
  'Accept': 'application/json'
}; 
