import { useState, useCallback } from 'react';
import api from '../services/api';
import useErrorHandler from './useErrorHandler';

/**
 * Интерфейс ответа API
 */
interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  errorMessage?: string;
  statusCode?: number;
  errorCode?: string;   // Код ошибки из бэкенда
  errorType?: string;   // Тип ошибки из бэкенда
}

/**
 * Хук для удобной работы с API запросами
 * Включает обработку ошибок и состояния загрузки
 */
export const useApi = () => {
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { handleError } = useErrorHandler();

  /**
   * Безопасный GET запрос с обработкой ошибок
   */
  const safeGet = useCallback(async <T,>(url: string, params?: object): Promise<ApiResponse<T>> => {
    setIsLoading(true);
    
    try {
      const response = await api.get<T>(url, { params });
      
      setIsLoading(false);
      return {
        success: true,
        data: response.data,
        statusCode: response.status
      };
    } catch (error: any) {
      setIsLoading(false);
      
      handleError(error);
      
      return {
        success: false,
        data: null,
        errorMessage: error.response?.data?.message || error.message || 'Ошибка при выполнении запроса'
      };
    }
  }, [handleError]);

  /**
   * Безопасный POST запрос с обработкой ошибок
   */
  const safePost = useCallback(async <T,>(url: string, data?: any): Promise<ApiResponse<T>> => {
    setIsLoading(true);
    
    try {
      const response = await api.post<T>(url, data);
      
      setIsLoading(false);
      return {
        success: true,
        data: response.data,
        statusCode: response.status
      };
    } catch (error: any) {
      setIsLoading(false);
      
      // Просто передаем ошибку с бэкенда без обработки
      // Извлекаем оригинальные поля из ответа сервера
      const responseData = error.response?.data || {};
      
      return {
        success: false,
        data: null,
        statusCode: error.response?.status,
        errorMessage: responseData.message || responseData.detail || responseData.title || error.message,
        errorCode: responseData.errorCode,
        errorType: responseData.errorType
      };
    }
  }, []);

  /**
   * Безопасный PUT запрос с обработкой ошибок
   */
  const safePut = useCallback(async <T,>(url: string, data?: any): Promise<ApiResponse<T>> => {
    setIsLoading(true);
    
    try {
      const response = await api.put<T>(url, data);
      
      setIsLoading(false);
      return {
        success: true,
        data: response.data,
        statusCode: response.status
      };
    } catch (error: any) {
      setIsLoading(false);
      
      handleError(error);
      
      return {
        success: false,
        data: null,
        errorMessage: error.response?.data?.message || error.message || 'Ошибка при обновлении данных'
      };
    }
  }, [handleError]);

  /**
   * Безопасный DELETE запрос с обработкой ошибок
   */
  const safeDelete = useCallback(async <T,>(url: string): Promise<ApiResponse<T>> => {
    setIsLoading(true);
    
    try {
      const response = await api.delete<T>(url);
      
      setIsLoading(false);
      return {
        success: true,
        data: response.data,
        statusCode: response.status
      };
    } catch (error: any) {
      setIsLoading(false);
      
      handleError(error);
      
      return {
        success: false,
        data: null,
        errorMessage: error.response?.data?.message || error.message || 'Ошибка при удалении'
      };
    }
  }, [handleError]);

  return {
    isLoading,
    safeGet,
    safePost,
    safePut,
    safeDelete
  };
};

export default useApi; 