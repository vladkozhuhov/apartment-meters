import api from './api';

// Интерфейс для запроса входа
export interface LoginRequest {
  username: string; // номер квартиры
  password: string;
}

// Интерфейс для ответа сервера аутентификации
export interface LoginResponse {
  token: string;
  username: string;
  roles: string[];
  expiration: string;
  errorCode?: string;    // Код ошибки из бэкенда
  errorType?: string;    // Тип ошибки из бэкенда
  errorMessage?: string; // Текстовое сообщение об ошибке
}

// Добавляем интерфейс для расширенной ошибки
interface EnhancedError extends Error {
  response?: any;
}

// Функция для входа в систему
export const login = async (loginData: LoginRequest): Promise<LoginResponse> => {
  try {
    const response = await api.post('/auth/login', loginData);
    
    // Проверяем, что ответ - это не HTML (например, страница 404)
    if (typeof response.data === 'string' && 
        (response.data.includes('<!DOCTYPE html>') || 
         response.data.includes('<html>'))) {
      console.error('Получен HTML-ответ вместо данных авторизации');
      throw new Error('Неверный номер квартиры или пароль');
    }
    
    return response.data;
  } catch (error: any) {
    console.error('Ошибка при выполнении запроса авторизации:', error);
    
    // Проверяем, содержит ли ответ HTML (страница 404)
    if (error.response && typeof error.response.data === 'string' && 
        (error.response.data.includes('<!DOCTYPE html>') || 
         error.response.data.includes('<html>'))) {
      console.error('Получен HTML-ответ в ошибке авторизации');
      throw new Error('Неверный номер квартиры или пароль');
    }
    
    // Ошибка 404 (сервер не найден)
    if (error.response && error.response.status === 404) {
      throw new Error('Неверный номер квартиры или пароль');
    }
    
    // Ошибка 401 (неавторизован)
    if (error.response && error.response.status === 401) {
      throw new Error('Неверный номер квартиры или пароль');
    }
    
    // Ошибка 400 (неверный запрос)
    if (error.response && error.response.status === 400) {
      throw new Error('Неверный формат данных для входа');
    }
    
    // Другие ошибки ответа сервера
    if (error.response && error.response.status) {
      const errorMessage = 
        error.response.data?.Message || 
        error.response.data?.message || 
        error.response.data?.detail || 
        error.response.data?.title || 
        'Неверный номер квартиры или пароль';
      
      const enhancedError: EnhancedError = new Error(errorMessage);
      enhancedError.name = 'AuthenticationError';
      enhancedError.response = error.response;
      throw enhancedError;
    }
    
    // Ошибки сети или другие ошибки
    throw new Error('Сервер недоступен. Пожалуйста, проверьте соединение и повторите попытку.');
  }
};

// Функция для проверки текущего пользователя
export const getCurrentUser = async () => {
  try {
    const response = await api.get('/api/auth/me');
    return response.data;
  } catch (error) {
    return null;
  }
};

// Функция для проверки, аутентифицирован ли пользователь
export const isAuthenticated = (): boolean => {
  try {
    // Проверяем, что мы в браузере, а не на сервере
    if (typeof window === 'undefined') {
      console.log('isAuthenticated: window не определен, запуск на сервере');
      return false;
    }
    
    const token = localStorage.getItem('token');
    if (!token) {
      console.log('Аутентификация не пройдена: токен отсутствует');
      return false;
    }
    
    // Проверка срока действия токена
    const expiration = localStorage.getItem('expiration');
    if (!expiration) {
      console.log('Аутентификация не пройдена: срок действия не найден');
      return false;
    }
    
    const expirationDate = new Date(expiration);
    const now = new Date();
    
    // Добавляем допуск 1 минуту для компенсации разницы во времени между клиентом и сервером
    const bufferTime = 1 * 60 * 1000; // 1 минута в миллисекундах
    if (expirationDate.getTime() - now.getTime() < -bufferTime) {
      // Токен истек, очищаем хранилище
      console.log('Аутентификация не пройдена: токен истек');
      logout();
      return false;
    }
    
    return true;
  } catch (error) {
    console.error('Ошибка при проверке аутентификации:', error);
    return false;
  }
};

// Функция для выхода из системы
export const logout = (): void => {
  if (typeof window === 'undefined') return;
  
  localStorage.removeItem('token');
  localStorage.removeItem('id');
  localStorage.removeItem('username');
  localStorage.removeItem('expiration');
  localStorage.removeItem('roles');
  localStorage.removeItem('apartmentNumber');
};

// Функция для сохранения данных аутентификации
export const saveAuthData = (data: LoginResponse): void => {
  try {
    if (!data.token) {
      console.error('Ошибка сохранения данных аутентификации: отсутствует токен');
      return;
    }
    
    console.log('Сохранение данных аутентификации...', data);
    
    // Сохраняем основные данные аутентификации
    localStorage.setItem('token', data.token);
    localStorage.setItem('username', data.username);
    localStorage.setItem('expiration', data.expiration);
    localStorage.setItem('roles', JSON.stringify(data.roles || []));
    
    // Извлекаем ID пользователя из токена JWT (если возможно)
    try {
      const tokenParts = data.token.split('.');
      if (tokenParts.length === 3) {
        const payload = JSON.parse(atob(tokenParts[1]));
        console.log('Данные из токена:', payload);
        
        if (payload.sub) {
          localStorage.setItem('id', payload.sub);
          console.log('ID пользователя сохранен:', payload.sub);
        }
      }
    } catch (tokenError) {
      console.warn('Не удалось извлечь ID из токена:', tokenError);
    }
    
    console.log('Проверка сохраненных данных:');
    console.log('token:', localStorage.getItem('token') ? 'задан' : 'не задан');
    console.log('username:', localStorage.getItem('username'));
    console.log('id:', localStorage.getItem('id'));
    console.log('expiration:', localStorage.getItem('expiration'));
    console.log('roles:', localStorage.getItem('roles'));
    
    console.log('Данные аутентификации успешно сохранены');
  } catch (error) {
    console.error('Ошибка при сохранении данных аутентификации:', error);
  }
};

// Функция для получения ролей пользователя
export const getUserRoles = (): string[] => {
  const rolesJson = localStorage.getItem('roles');
  if (!rolesJson) return [];
  try {
    return JSON.parse(rolesJson);
  } catch {
    return [];
  }
};

// Функция для проверки, имеет ли пользователь определенную роль
export const hasRole = (role: string): boolean => {
  const roles = getUserRoles();
  return roles.includes(role);
}; 
