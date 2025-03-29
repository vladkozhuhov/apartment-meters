import React from 'react';
import { NextPage } from 'next';
import { ErrorProps } from 'next/error';

// Пользовательская страница ошибки для замены стандартной Next.js
const CustomErrorPage: NextPage<ErrorProps> = ({ statusCode }) => {
  return (
    <div className="min-h-screen flex flex-col items-center justify-center bg-gray-50 p-4">
      <div className="bg-white p-6 rounded-lg shadow-md max-w-md w-full text-center">
        <h1 className="text-xl font-bold text-gray-800 mb-2">
          Произошла ошибка
        </h1>
        
        <p className="text-gray-600 mb-4">
          {statusCode 
            ? `Ошибка ${statusCode}: Не удалось выполнить запрос на сервер`
            : 'Произошла непредвиденная ошибка в приложении'
          }
        </p>
        
        <div className="space-y-2">
          <button
            onClick={() => window.location.href = '/login'}
            className="w-full py-2 px-4 bg-blue-600 text-white rounded hover:bg-blue-700"
          >
            Вернуться на страницу входа
          </button>
          
          <button
            onClick={() => window.location.reload()}
            className="w-full py-2 px-4 border border-gray-300 bg-white text-gray-700 rounded hover:bg-gray-50"
          >
            Обновить страницу
          </button>
        </div>
      </div>
    </div>
  );
};

export default CustomErrorPage;
