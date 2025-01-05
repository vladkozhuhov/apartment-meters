import { useState } from 'react';
import { useRouter } from 'next/router';
import api from '../services/api';

const LoginPage: React.FC = () => {
  const [apartmentNumber, setApartmentNumber] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const router = useRouter();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(''); // Очистка предыдущих ошибок

    try {
      const response = await api.post('/api/Auth/login', 
        {
          apartmentNumber: Number(apartmentNumber), // Преобразуем строку в число
          password,
        }, 
        {
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );

      if (response.status === 200) {
        const userData = response.data;

        // Проверяем роль пользователя
        if (userData.role === 0) {
          localStorage.setItem('id', response.data.id); // Сохраняем userId в localStorage
          router.push('/user'); // Переход на страницу пользователя
        } else if (userData.role === 1) {
          router.push('/admin'); // Переход на страницу админа
        }
      }
    } catch (err: any) {
      console.error('Ошибка авторизации:', err);
      setError('Неверный номер квартиры или пароль.');
    }
  };

  return (
    <form onSubmit={handleLogin} className="flex flex-col space-y-4">
      <div className="flex min-h-full flex-1 flex-col justify-center px-6 py-12 lg:px-8">
        <div className="sm:mx-auto sm:w-full sm:max-w-sm">
          {/* <!-- <img
            alt="Your Company"
            src="https://tailwindui.com/plus/img/logos/mark.svg?color=indigo&shade=600"
            className="mx-auto h-10 w-auto"
          /> --> */}
          <h2 className="mt-10 text-center text-2xl/9 font-bold tracking-tight text-gray-900">
            Вход в свой аккаунт
          </h2>
        </div>

        <div className="mt-10 sm:mx-auto sm:w-full sm:max-w-sm">
          <div className="space-y-6">
            <div>
              <label htmlFor="apartmentNumber" className="block text-sm/6 font-medium text-gray-900">
                Номер квартиры:
              </label>
              <div className="mt-2">
                <input
                  type="number"
                  id="apartmentNumber"
                  name="apartmentNumber"
                  placeholder="Введите ваш номер квартиры"
                  value={apartmentNumber}
                  onChange={(e) => setApartmentNumber(e.target.value)}
                  required
                  className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
                />
              </div>
            </div>

            <div>
              <div className="flex items-center justify-between">
                <label htmlFor="password" className="block text-sm/6 font-medium text-gray-900">
                  Пароль:
                </label>
              </div>
              <div className="mt-2">
                <input
                  type="password"
                  id="password"
                  name="password"
                  placeholder="Введите ваш пароль"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="block w-full rounded-md bg-white px-3 py-1.5 text-base text-gray-900 outline outline-1 -outline-offset-1 outline-gray-300 placeholder:text-gray-400 focus:outline focus:outline-2 focus:-outline-offset-2 focus:outline-indigo-600 sm:text-sm/6"
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                className="flex w-full justify-center rounded-md bg-indigo-600 px-3 py-1.5 text-sm/6 font-semibold text-white shadow-sm hover:bg-indigo-500 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-indigo-600"
              >
                Вход
              </button>
            </div>
          </div>

          {error && (
            <p className="text-red-500 text-sm font-medium mt-2">{error}</p>
          )}

        </div>
      </div>
    </form>
  );
};

export default LoginPage;