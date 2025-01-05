import React, { useEffect } from 'react';
import { useRouter } from 'next/router';

const IndexPage: React.FC = () => {
    const router = useRouter();

    useEffect(() => {
        if (typeof window !== 'undefined') {
            const isAuthenticated = localStorage.getItem('token'); // Проверяем наличие токена
            if (isAuthenticated) {
                router.push('/user'); // Если пользователь авторизован, перенаправляем на /user
            } else {
                router.push('/login'); // Иначе перенаправляем на /login
            }
        }
    }, [router]);

    return (
        <div className="h-screen flex items-center justify-center">
            <h1 className="text-2xl font-bold">Загрузка...</h1>
        </div>
    );
};

export default IndexPage;