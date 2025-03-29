import { useState, useEffect } from 'react';
import { useRouter } from 'next/router';
import { LoginRequest, LoginResponse, saveAuthData } from '../services/authService';
import dynamic from 'next/dynamic';
import { useError } from '../contexts/ErrorContext';
import { useApi } from '../hooks/useApi';
import { ErrorType } from '../hooks/useErrorHandler';

// Коды ошибок авторизации
const AUTH_ERROR_CODES = {
  USER_NOT_FOUND: ErrorType.UserNotFoundError101,
  INVALID_PASSWORD: ErrorType.InvalidPasswordError102,
  USER_BLOCKED: ErrorType.UserBlockedError103
};

// Простая функция для отображения браузерного alert в режиме разработки
const debugAlert = (message: string) => {
  if (process.env.NODE_ENV === 'development') {
    console.warn(`[DEBUG ALERT] ${message}`);
    // Раскомментируйте следующую строку для отладки
    // alert(message);
  }
};

// Делаем компонент доступным только на клиенте, без SSR
const LoginPage = () => {
  const [apartmentNumber, setApartmentNumber] = useState('');
  const [password, setPassword] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [loginError, setLoginError] = useState<string | null>(null);
  const [showPassword, setShowPassword] = useState(false); // Состояние для отображения пароля
  const router = useRouter();
  const { showError, clearError } = useError();
  const api = useApi();

  // Очищаем ошибки при монтировании
  useEffect(() => {
    // Просто очищаем ошибки
    clearError();
    setLoginError(null);
  }, [clearError]);

  // Переключение видимости пароля
  const togglePasswordVisibility = () => {
    setShowPassword(!showPassword);
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    
    setLoginError('');
    
    // Проверка валидности данных
    if (!apartmentNumber) {
      setLoginError('Введите номер квартиры');
      return;
    }
    
    if (!password) {
      setLoginError('Введите пароль');
      return;
    }
    
    setIsLoading(true);
    
    try {
      const response = await api.safePost<LoginResponse>('/api/auth/login', {
        username: apartmentNumber,
        password
      });
      
      if (response.success && response.data) {
        // Сохраняем данные авторизации
        saveAuthData(response.data);
        
        // Определяем, куда перенаправить пользователя
        const roles = response.data.roles || [];
        const redirectUrl = roles.includes('Admin') ? '/admin' : '/user';
        
        // Перенаправляем пользователя
        router.push(redirectUrl);
      } else {
        // Просто показываем оригинальную ошибку с бэкенда
        setLoginError(response.errorMessage || 'Не удалось выполнить вход');
      }
    } catch (error: any) {
      // Извлекаем чистое сообщение с бэкенда без обработки
      if (error.response?.data?.message) {
        setLoginError(error.response.data.message);
      } else if (error.response?.data?.detail) {
        setLoginError(error.response.data.detail);
      } else {
        setLoginError('Не удалось выполнить вход');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-50 via-white to-indigo-50 flex flex-col justify-start py-8 sm:px-6 lg:px-8">
      <div className="sm:mx-auto sm:w-full sm:max-w-md">
        <div className="text-center">
          <h2 className="text-3xl font-extrabold text-gray-900 mb-2">
            Вход в систему
          </h2>
          <p className="text-gray-600 text-lg">
            Введите данные вашей квартиры
          </p>
        </div>
      </div>

      <div className="mt-6 sm:mx-auto sm:w-full sm:max-w-md">
        <div className="bg-white py-8 px-4 shadow-2xl sm:rounded-lg sm:px-10 border border-gray-100">
          {/* Показываем ошибку, если она есть */}
          {loginError && (
            <div className="mb-4 bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md shadow-md" role="alert">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <svg className="h-5 w-5 text-red-500" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
                  </svg>
                </div>
                <div className="ml-3">
                  <p className="text-sm font-medium text-red-800">
                    <strong>Ошибка: </strong>
                    <span>{loginError}</span>
                  </p>
                </div>
              </div>
            </div>
          )}
          
          <form onSubmit={handleLogin} className="space-y-6">
            <div>
              <label htmlFor="apartmentNumber" className="block text-sm font-medium text-gray-700">
                Номер квартиры
              </label>
              <div className="mt-1">
                <input
                  type="number"
                  id="apartmentNumber"
                  name="apartmentNumber"
                  placeholder="Введите номер квартиры"
                  value={apartmentNumber}
                  onChange={(e) => setApartmentNumber(e.target.value)}
                  required
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
                  disabled={isLoading}
                />
              </div>
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                Пароль
              </label>
              <div className="mt-1 relative">
                <input
                  type={showPassword ? "text" : "password"}
                  id="password"
                  name="password"
                  placeholder="Введите пароль"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="appearance-none block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm placeholder-gray-400 focus:outline-none focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm pr-10"
                  disabled={isLoading}
                />
                <button
                  type="button"
                  onClick={togglePasswordVisibility}
                  className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-500 hover:text-gray-700 focus:outline-none"
                >
                  {showPassword ? (
                    // Иконка "скрыть пароль" (глаз перечеркнутый)
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                      <path fillRule="evenodd" d="M3.707 2.293a1 1 0 00-1.414 1.414l14 14a1 1 0 001.414-1.414l-1.473-1.473A10.014 10.014 0 0019.542 10C18.268 5.943 14.478 3 10 3a9.958 9.958 0 00-4.512 1.074l-1.78-1.781zm4.261 4.26l1.514 1.515a2.003 2.003 0 012.45 2.45l1.514 1.514a4 4 0 00-5.478-5.478z" clipRule="evenodd" />
                      <path d="M12.454 16.697L9.75 13.992a4 4 0 01-3.742-3.741L2.335 6.578A9.98 9.98 0 00.458 10c1.274 4.057 5.065 7 9.542 7 .847 0 1.669-.105 2.454-.303z" />
                    </svg>
                  ) : (
                    // Иконка "показать пароль" (глаз)
                    <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                      <path d="M10 12a2 2 0 100-4 2 2 0 000 4z" />
                      <path fillRule="evenodd" d="M.458 10C1.732 5.943 5.522 3 10 3s8.268 2.943 9.542 7c-1.274 4.057-5.064 7-9.542 7S1.732 14.057.458 10zM14 10a4 4 0 11-8 0 4 4 0 018 0z" clipRule="evenodd" />
                    </svg>
                  )}
                </button>
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={isLoading}
                className={`w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white ${
                  isLoading ? 'bg-indigo-400' : 'bg-indigo-600 hover:bg-indigo-700'
                } focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 transition-colors duration-200`}
              >
                {isLoading ? (
                  <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : null}
                {isLoading ? 'Вход...' : 'Войти'}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};

// Экспортируем компонент с отключенным SSR
export default dynamic(() => Promise.resolve(LoginPage), { ssr: false });