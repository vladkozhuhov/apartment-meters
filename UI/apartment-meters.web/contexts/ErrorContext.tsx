import React, { createContext, useContext, useState, ReactNode, useCallback } from 'react';

// Типы для системы обработки ошибок
export type ErrorSeverity = 'error' | 'warning' | 'info' | 'success';

interface ErrorContextType {
  error: string | null;
  severity: ErrorSeverity;
  showError: (message: string, severity?: ErrorSeverity) => void;
  clearError: () => void;
}

// Создаем контекст с начальными значениями
const ErrorContext = createContext<ErrorContextType>({
  error: null,
  severity: 'error',
  showError: () => {},
  clearError: () => {}
});

// Хук для использования контекста ошибок
export const useError = () => useContext(ErrorContext);

interface ErrorProviderProps {
  children: ReactNode;
}

// Провайдер контекста ошибок
export const ErrorProvider: React.FC<ErrorProviderProps> = ({ children }) => {
  const [error, setError] = useState<string | null>(null);
  const [severity, setSeverity] = useState<ErrorSeverity>('error');

  // Функция для отображения ошибки с указанием типа
  const showError = useCallback((message: string, errorSeverity: ErrorSeverity = 'error') => {
    setError(message);
    setSeverity(errorSeverity);
  }, []);

  // Функция для очистки ошибки
  const clearError = useCallback(() => {
    setError(null);
  }, []);

  // Предоставляем доступ к состоянию и функциям обработки ошибок
  const contextValue = {
    error,
    severity,
    showError,
    clearError
  };

  return (
    <ErrorContext.Provider value={contextValue}>
      {children}
    </ErrorContext.Provider>
  );
};

export default ErrorProvider; 