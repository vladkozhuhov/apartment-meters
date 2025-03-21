import axios from 'axios';

const api = axios.create({
  baseURL: `http://localhost:8080`,
});

// Добавляем перехватчик для добавления токена к запросам
api.interceptors.request.use(
  config => {
    console.log('Выполняется запрос:', config.method?.toUpperCase(), config.url);
    
    const token = localStorage.getItem('token');
    if (token) {
      console.log('Токен авторизации добавлен к запросу');
      config.headers['Authorization'] = `Bearer ${token}`;
    } else {
      console.log('Токен авторизации отсутствует');
    }
    return config;
  },
  error => {
    console.error('Ошибка при выполнении запроса:', error);
    return Promise.reject(error);
  }
);

// Добавляем перехватчик для обработки ответов
api.interceptors.response.use(
  response => {
    console.log('Получен ответ:', response.status, response.config.url);
    // Логируем тело ответа только если это не бинарные данные
    if (response.headers['content-type']?.includes('application/json')) {
      console.log('Тело ответа:', response.data);
    }
    return response;
  },
  error => {
    // Если сервер вернул 401 Unauthorized - перенаправляем на страницу логина
    if (error.response) {
      console.error('Ошибка ответа:', error.response.status, error.config.url);
      console.error('Детали ошибки:', error.response.data);
      
      if (error.response.status === 401) {
        console.log('Получен статус 401 Unauthorized, перенаправление на /login');
        localStorage.removeItem('token');
        localStorage.removeItem('id');
        localStorage.removeItem('apartmentNumber');
        
        // Перенаправляем на страницу логина, если это не страница логина
        if (typeof window !== 'undefined' && window.location.pathname !== '/login') {
          window.location.href = '/login';
        }
      }
    } else {
      console.error('Ошибка сети:', error.message);
    }
    return Promise.reject(error);
  }
);

export default api;