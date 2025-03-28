import React, { useEffect } from 'react';
import { useRouter } from 'next/router';
import { isAuthenticated } from '../services/authService';

const IndexPage: React.FC = () => {
    const router = useRouter();

    useEffect(() => {
        if (typeof window !== 'undefined') {
            if (isAuthenticated()) {
                // Определяем роль пользователя для перенаправления
                const userRole = localStorage.getItem('role');
                const redirectUrl = userRole === 'Admin' ? '/admin' : '/user';
                
                // Используем router.push без опции replace
                router.push(redirectUrl);
            } else {
                router.push('/login');
            }
        }
    }, [router]);

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex flex-col items-center justify-center">
            <div className="text-center">
                <div className="inline-block animate-spin rounded-full h-16 w-16 border-t-2 border-b-2 border-indigo-600 mb-4"></div>
                <h1 className="text-2xl font-semibold text-gray-800 mb-2">Загрузка приложения</h1>
                <p className="text-gray-600">Пожалуйста, подождите...</p>
            </div>
        </div>
    );
};

export default IndexPage;