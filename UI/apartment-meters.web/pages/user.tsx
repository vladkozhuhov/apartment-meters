"use client"

import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/router';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';
import { getMeterReadingByWaterMeterId } from '../services/readingMeterService';
import AddMeterReadingForm from '@/components/AddMeterReadingComponent';
import { isAuthenticated, logout, getCurrentUser } from '../services/authService';
import { getUserByApartmentNumber } from '../services/userService';
import api from '../services/api'; 
import { useError } from '../contexts/ErrorContext';

interface MeterReading {
  id: string;
  waterValue: string;
  differenceValue: number;
  readingDate: Date;
  meterId?: string;
  placeOfWaterMeter?: number;
  waterType?: number;
}

interface WaterMeter extends WaterMeterRequest {
  readings: MeterReading[];
}

interface CombinedReading {
  date: Date;
  bathroomHot: string;
  bathroomHotDiff: string;
  bathroomCold: string;
  bathroomColdDiff: string;
  kitchenHot: string;
  kitchenHotDiff: string;
  kitchenCold: string;
  kitchenColdDiff: string;
}

const UserPage: React.FC = () => {
  const [waterMeters, setWaterMeters] = useState<WaterMeter[]>([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const router = useRouter();
  const [apartmentNumber, setApartmentNumber] = useState<string | null>(null);
  const [userId, setUserId] = useState<string | null>(null);
  const [isClient, setIsClient] = useState(false);
  const { showError, clearError } = useError();

  // Устанавливаем флаг клиентского рендеринга
  useEffect(() => {
    setIsClient(true);
  }, []);

  // Проверяем аутентификацию при загрузке страницы
  useEffect(() => {
    // Проверяем, что мы на клиенте
    if (!isClient) return;
    
    console.log('Проверка аутентификации на странице пользователя');
    const authStatus = isAuthenticated();
    console.log('Результат проверки аутентификации:', authStatus);
    
    if (!authStatus) {
      console.log('Перенаправление на страницу входа из-за отсутствия аутентификации');
      router.push('/login');
      return;
    }
    
    // Получаем информацию о текущем пользователе
    const fetchCurrentUser = async () => {
      try {
        console.log('Получение информации о текущем пользователе');
        const user = await getCurrentUser();
        console.log('Данные о пользователе получены:', user);
        
        // Проверяем аутентификацию пользователя, даже если userId отсутствует
        if (user && user.isAuthenticated) {
          // Используем apartmentNumber для загрузки пользователя, если userId отсутствует
          if (!user.userId && user.username) {
            console.log('userId отсутствует, но пользователь аутентифицирован. Получаем данные по номеру квартиры:', user.username);
            
            // Устанавливаем номер квартиры для отображения
            setApartmentNumber(user.username);
            localStorage.setItem('apartmentNumber', user.username);
            
            // Получаем пользователя по номеру квартиры
            try {
              const userData = await getUserByApartmentNumber(user.username);
              if (userData && userData.id) {
                setUserId(userData.id);
                localStorage.setItem('id', userData.id);
                console.log('Получен userId по номеру квартиры:', userData.id);
              } else {
                console.warn('Не удалось получить ID пользователя по номеру квартиры');
              }
            } catch (err) {
              console.error('Ошибка при получении данных пользователя по номеру квартиры:', err);
            }
          } else if (user.userId) {
            // Стандартный путь, если userId есть
            setUserId(user.userId);
            localStorage.setItem('id', user.userId);
            
            // Если существует username в ответе, устанавливаем его как номер квартиры
            if (user.username) {
              setApartmentNumber(user.username);
              localStorage.setItem('apartmentNumber', user.username);
            }
          }
        } else {
          // Если информация о пользователе не получена, перенаправляем на страницу входа
          console.log('Информация о пользователе не получена, выполняем выход');
          logout();
          router.push('/login');
        }
      } catch (err) {
        console.error('Ошибка при получении данных пользователя:', err);
        showError('Не удалось загрузить данные пользователя', 'error');
        // При ошибке также выполняем выход
        logout();
        router.push('/login');
      }
    };
    
    fetchCurrentUser();
  }, [router, isClient]);

  const combineReadings = (meters: WaterMeter[]): CombinedReading[] => {
    const allReadings: CombinedReading[] = [];
    
    // Создаем Map для группировки показаний по сессиям
    const readingsBySession = new Map<string, {
      date: Date;
      bathroomHot: string;
      bathroomHotDiff: string;
      bathroomCold: string;
      bathroomColdDiff: string;
      kitchenHot: string;
      kitchenHotDiff: string;
      kitchenCold: string;
      kitchenColdDiff: string;
    }>();

    // Сначала сгруппируем все показания по сессиям
    const allMeterReadings: Array<{
      reading: MeterReading;
      meter: WaterMeter;
    }> = [];

    meters.forEach(meter => {
      meter.readings.forEach(reading => {
        allMeterReadings.push({
          reading,
          meter
        });
      });
    });

    // Сортируем все показания по времени
    allMeterReadings.sort((a, b) => {
      const timeA = new Date(a.reading.readingDate).getTime();
      const timeB = new Date(b.reading.readingDate).getTime();
      return timeA - timeB;
    });

    // Группируем показания в сессии
    let currentSession: {
      date: Date;
      bathroomHot: string;
      bathroomHotDiff: string;
      bathroomCold: string;
      bathroomColdDiff: string;
      kitchenHot: string;
      kitchenHotDiff: string;
      kitchenCold: string;
      kitchenColdDiff: string;
    } | null = null;

    allMeterReadings.forEach(({ reading, meter }) => {
      const readingDate = new Date(reading.readingDate);
      const isKitchen = meter.placeOfWaterMeter === 1;
      const isHot = meter.waterType === 1;

      // Если это первое показание или прошло больше минуты с предыдущего,
      // создаем новую сессию
      if (!currentSession || 
          Math.abs(readingDate.getTime() - new Date(currentSession.date).getTime()) > 60000) {
        if (currentSession) {
          allReadings.push(currentSession);
        }
        currentSession = {
          date: readingDate,
          bathroomHot: '-',
          bathroomHotDiff: '-',
          bathroomCold: '-',
          bathroomColdDiff: '-',
          kitchenHot: '-',
          kitchenHotDiff: '-',
          kitchenCold: '-',
          kitchenColdDiff: '-',
        };
      }

      // Добавляем показание в текущую сессию
      if (isKitchen) {
        if (isHot) {
          currentSession.kitchenHot = reading.waterValue;
          currentSession.kitchenHotDiff = reading.differenceValue.toFixed(3);
        } else {
          currentSession.kitchenCold = reading.waterValue;
          currentSession.kitchenColdDiff = reading.differenceValue.toFixed(3);
        }
      } else {
        if (isHot) {
          currentSession.bathroomHot = reading.waterValue;
          currentSession.bathroomHotDiff = reading.differenceValue.toFixed(3);
        } else {
          currentSession.bathroomCold = reading.waterValue;
          currentSession.bathroomColdDiff = reading.differenceValue.toFixed(3);
        }
      }
    });

    // Добавляем последнюю сессию
    if (currentSession) {
      allReadings.push(currentSession);
    }

    // Сортируем по дате по возрастанию (старые записи сверху, новые снизу)
    return allReadings.sort((a, b) => a.date.getTime() - b.date.getTime());
  };

  const fetchUserWaterMeters = async () => {
    if (!userId) {
      return; // Ждем, пока userId будет установлен
    }

    try {
      setLoading(true);
      const meters = await getWaterMetersByUserId(userId as string);
      const metersWithReadings = await Promise.all(
        meters.map(async (meter: WaterMeterRequest) => {
          const readings = await getMeterReadingByWaterMeterId(meter.id);
          return { ...meter, readings };
        })
      );
      setWaterMeters(metersWithReadings);
    } catch (err: any) {
      console.error('Ошибка при получении данных о счетчиках:', err);
      // Если ошибка 401 - перенаправляем на страницу входа
      if (err.response && err.response.status === 401) {
        logout();
        router.push('/login');
        return;
      }
      showError('Не удалось загрузить данные. Пожалуйста, попробуйте позже.', 'error');
    } finally {
      setLoading(false);
    }
  };

  // Загружаем данные счетчиков при изменении userId
  useEffect(() => {
    if (userId && isClient) {
      fetchUserWaterMeters();
      clearError(); // Очищаем ошибки при загрузке данных
    }
  }, [userId, isClient, clearError]);

  const handleLogout = () => {
    logout();
    router.push('/login');
  };

  // Если страница еще не загружена на клиенте, показываем пустую разметку,
  // чтобы избежать различий между серверным и клиентским рендерингом
  if (!isClient) {
    return <div className="min-h-screen flex items-center justify-center"></div>;
  }

  // Показываем загрузку, пока не получили userId
  if (loading && !userId) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="text-center">
          <div className="inline-block animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-indigo-600 mb-3"></div>
          <p className="text-gray-600">Загрузка данных...</p>
        </div>
      </div>
    );
  }

  return (
    <div className="p-2 sm:p-5 w-full max-w-full sm:max-w-7xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-6 sm:mb-8">
        <div className="flex justify-between items-start mb-4 sm:mb-6">
          <div className="flex-1">
            <h1 className="text-xl sm:text-2xl font-bold text-gray-800 mb-4">Личный кабинет</h1>
          </div>

          {/* Правая часть с кнопкой */}
          <div>
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-500 text-white px-4 py-2 sm:px-6 sm:py-3 rounded-lg hover:bg-blue-600 transition-colors duration-200 flex items-center text-sm sm:text-base"
            >
              <svg className="w-4 h-4 sm:w-5 sm:h-5 mr-1 sm:mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Добавить показания
            </button>
          </div>
        </div>

        {/* Информация о квартире */}
        <div className="bg-blue-50 p-3 sm:p-4 rounded-lg mb-4 sm:mb-6 text-sm sm:text-base">
          <p className="text-gray-700">г. Магнитогорск</p>
          <p className="text-gray-700">пр-т Ленина, 90</p>
          <p className="font-medium text-gray-800">{apartmentNumber ? `Квартира ${apartmentNumber}` : 'Загрузка...'}</p>
        </div>

        {/* Информация о счетчиках */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3 sm:gap-4">
          <div>
            <h2 className="text-base sm:text-lg font-semibold text-gray-800 mb-2 sm:mb-3">Счетчики в ванной</h2>
            <div className="space-y-2 sm:space-y-4">
              {waterMeters.filter(meter => meter.placeOfWaterMeter === 0).map(meter => (
                <div key={meter.id} className="bg-gray-50 p-3 sm:p-4 rounded-lg border border-gray-200">
                  <div className="flex items-center mb-1 sm:mb-2">
                    <div className={`w-3 h-3 rounded-full mr-2 ${meter.waterType === 1 ? 'bg-red-500' : 'bg-blue-500'}`} />
                    <span className="font-medium text-gray-800 text-sm sm:text-base">
                      {meter.waterType === 1 ? 'Горячая вода' : 'Холодная вода'}
                    </span>
                  </div>
                  <div className="text-xs sm:text-sm text-gray-600">
                    <p>Заводской номер: {meter.factoryNumber}</p>
                    <p>Дата установки: {new Date(meter.factoryYear).toLocaleDateString()}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {waterMeters.some(meter => meter.placeOfWaterMeter === 1) && (
            <div>
              <h2 className="text-base sm:text-lg font-semibold text-gray-800 mb-2 sm:mb-3">Счетчики на кухне</h2>
              <div className="space-y-2 sm:space-y-4">
                {waterMeters.filter(meter => meter.placeOfWaterMeter === 1).map(meter => (
                  <div key={meter.id} className="bg-gray-50 p-3 sm:p-4 rounded-lg border border-gray-200">
                    <div className="flex items-center mb-1 sm:mb-2">
                      <div className={`w-3 h-3 rounded-full mr-2 ${meter.waterType === 1 ? 'bg-red-500' : 'bg-blue-500'}`} />
                      <span className="font-medium text-gray-800 text-sm sm:text-base">
                        {meter.waterType === 1 ? 'Горячая вода' : 'Холодная вода'}
                      </span>
                    </div>
                    <div className="text-xs sm:text-sm text-gray-600">
                      <p>Заводской номер: {meter.factoryNumber}</p>
                      <p>Дата установки: {new Date(meter.factoryYear).toLocaleDateString()}</p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>

      {showForm && (
        <AddMeterReadingForm
          userId={userId || ''}
          onSuccess={() => {
            setShowForm(false);
            fetchUserWaterMeters();
          }}
          onCancel={() => setShowForm(false)}
        />
      )}

      {loading ? (
        <div className="mt-4 bg-white rounded-lg shadow-md p-4 sm:p-6">
          <p>Загрузка данных...</p>
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
          <h2 className="text-lg sm:text-xl font-semibold mb-4 sm:mb-6">История показаний водомеров</h2>
          {waterMeters.length === 0 ? (
            <p>Пока нет данных о показаниях.</p>
          ) : (
            <div className="-mx-4 sm:mx-0 overflow-x-auto">
              <table className="w-full border-collapse border border-gray-300 text-xs sm:text-sm">
                <thead className="bg-gray-100">
                  <tr>
                    <th className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2" rowSpan={2}>Дата</th>
                    <th className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2" colSpan={4}>Ванная</th>
                    <th className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2" colSpan={4}>Кухня</th>
                  </tr>
                  <tr>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Горячая вода (м³)</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Потребление</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Холодная вода (м³)</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Потребление</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Горячая вода (м³)</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Потребление</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Холодная вода (м³)</th>
                    <th className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">Потребление</th>
                  </tr>
                </thead>
                <tbody>
                  {combineReadings(waterMeters).map((reading, index) => (
                    <tr key={index} className="text-center">
                      <td className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2">
                        {reading.date.toLocaleDateString()}
                      </td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.bathroomHot}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.bathroomHotDiff}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.bathroomCold}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.bathroomColdDiff}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.kitchenHot}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.kitchenHotDiff}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.kitchenCold}</td>
                      <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">{reading.kitchenColdDiff}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default UserPage;