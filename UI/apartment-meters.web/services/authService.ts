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
}

// Функция для входа в систему
export const login = async (loginData: LoginRequest): Promise<LoginResponse> => {
  const response = await api.post('/api/auth/login', loginData);
  return response.data;
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
    
    if (expirationDate < now) {
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