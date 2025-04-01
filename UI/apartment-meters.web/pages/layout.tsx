"use client";

import React, { ReactNode, useState, useEffect } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/router';
import { logout, isAuthenticated } from '../services/authService';
import PushNotificationComponent from '../components/PushNotificationComponent';

interface LayoutProps {
  children: ReactNode;
}

const Layout: React.FC<LayoutProps> = ({ children }) => {
  const router = useRouter();
  const [isClient, setIsClient] = useState(false);
  const [isAuth, setIsAuth] = useState(false);

  // Устанавливаем флаг клиентского рендеринга и проверяем аутентификацию
  useEffect(() => {
    setIsClient(true);
    setIsAuth(isAuthenticated());
  }, []);

  const handleLogout = () => {
    logout();
    router.push('/login');
  };

  return (
    <div className="flex flex-col min-h-screen">
      <header className="bg-gradient-to-r from-blue-700 via-blue-600 to-indigo-700 text-white py-5 px-8 shadow-lg relative overflow-hidden">
        <div 
          className="absolute inset-0 opacity-10"
          style={{
            backgroundImage: `url("data:image/svg+xml,%3Csvg width='30' height='30' viewBox='0 0 30 30' fill='none' xmlns='http://www.w3.org/2000/svg'%3E%3Cpath d='M1.22676 0C1.91374 0 2.45351 0.539773 2.45351 1.22676C2.45351 1.91374 1.91374 2.45351 1.22676 2.45351C0.539773 2.45351 0 1.91374 0 1.22676C0 0.539773 0.539773 0 1.22676 0Z' fill='rgba(255,255,255,0.07)'/%3E%3C/svg%3E")`,
            backgroundRepeat: 'repeat'
          }}
        />
        <div className="container mx-auto flex justify-between items-center relative z-10">
          <div className="flex items-center space-x-2">
            <svg className="h-8 w-8" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <path d="M12 2C6.48 2 2 6.48 2 12C2 17.52 6.48 22 12 22C17.52 22 22 17.52 22 12C22 6.48 17.52 2 12 2ZM12 20C7.58 20 4 16.42 4 12C4 7.58 7.58 4 12 4C16.42 4 20 7.58 20 12C20 16.42 16.42 20 12 20Z" fill="currentColor"/>
              <path d="M12 17C14.7614 17 17 14.7614 17 12C17 9.23858 14.7614 7 12 7C9.23858 7 7 9.23858 7 12C7 14.7614 9.23858 17 12 17Z" fill="currentColor"/>
            </svg>
            <h1 className="text-2xl font-bold tracking-tight">
              Учёт показаний счётчиков
            </h1>
          </div>
          <nav>
            <ul className="flex items-center space-x-8">
              {isClient ? (
                isAuth ? (
                  <>
                    <li>
                      <button
                        onClick={handleLogout}
                        className="flex items-center space-x-2 hover:text-blue-200 transition-colors duration-200"
                      >
                        <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                          <path fillRule="evenodd" d="M3 3a1 1 0 00-1 1v12a1 1 0 102 0V4a1 1 0 00-1-1zm10.293 9.293a1 1 0 001.414 1.414l3-3a1 1 0 000-1.414l-3-3a1 1 0 10-1.414 1.414L14.586 9H7a1 1 0 100 2h7.586l-1.293 1.293z" clipRule="evenodd" />
                        </svg>
                        <span>Выйти</span>
                      </button>
                    </li>
                  </>
                ) : (
                  <li>
                    <Link href="/login" className="flex items-center space-x-2 hover:text-blue-200 transition-colors duration-200">
                      <svg className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                        <path fillRule="evenodd" d="M3 3a1 1 0 011 1v12a1 1 0 11-2 0V4a1 1 0 011-1zm7.707 3.293a1 1 0 010 1.414L9.414 9H17a1 1 0 110 2H9.414l1.293 1.293a1 1 0 01-1.414 1.414l-3-3a1 1 0 010-1.414l3-3a1 1 0 011.414 0z" clipRule="evenodd" />
                      </svg>
                      <span>Войти</span>
                    </Link>
                  </li>
                )
              ) : (
                // На сервере показываем пустой nav, чтобы избежать несоответствия рендеринга
                <li className="opacity-0 h-5">
                  <span>&nbsp;</span>
                </li>
              )}
            </ul>
          </nav>
        </div>
      </header>
      <main className="flex-grow p-4">
        {isClient && isAuth && (
          <div className="mb-2">
            <PushNotificationComponent />
          </div>
        )}
        {children}
      </main>
      <footer className="bg-gray-800 text-white py-6 px-8">
        <div className="container mx-auto text-center">
          <p>&copy; {new Date().getFullYear()} Система учёта показаний счётчиков. Все права защищены.</p>
          <p className="text-gray-400 text-sm mt-2">Версия 1.0.0</p>
        </div>
      </footer>
    </div>
  );
};

export default Layout;