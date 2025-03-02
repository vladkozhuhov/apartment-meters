"use client"

import React, { useEffect, useState } from 'react';
import { useRouter } from 'next/router';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';
import { getMeterReadingByWaterMeterId } from '../services/readingMeterService';
import AddMeterReadingForm from '@/components/addMeterReadingFormComponent';

interface MeterReading {
  id: string;
  waterValue: string;
  totalValue: number;
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
  bathroomCold: string;
  bathroomTotal: string;
  bathroomDiff: string;
  kitchenHot: string;
  kitchenCold: string;
  kitchenTotal: string;
  kitchenDiff: string;
}

const UserPage: React.FC = () => {
  const [waterMeters, setWaterMeters] = useState<WaterMeter[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const router = useRouter();
  const [apartmentNumber, setApartmentNumber] = useState<string | null>(null);

  const userId = localStorage.getItem('id');

  const combineReadings = (meters: WaterMeter[]): CombinedReading[] => {
    const allReadings: CombinedReading[] = [];
    
    // Создаем Map для группировки показаний по сессиям
    const readingsBySession = new Map<string, {
      date: Date;
      bathroomHot: string;
      bathroomCold: string;
      bathroomTotal: string;
      bathroomDiff: string;
      kitchenHot: string;
      kitchenCold: string;
      kitchenTotal: string;
      kitchenDiff: string;
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
      bathroomCold: string;
      bathroomTotal: string;
      bathroomDiff: string;
      kitchenHot: string;
      kitchenCold: string;
      kitchenTotal: string;
      kitchenDiff: string;
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
          bathroomCold: '-',
          bathroomTotal: '-',
          bathroomDiff: '-',
          kitchenHot: '-',
          kitchenCold: '-',
          kitchenTotal: '-',
          kitchenDiff: '-'
        };
      }

      // Добавляем показание в текущую сессию
      if (isKitchen) {
        if (isHot) {
          currentSession.kitchenHot = reading.waterValue;
        } else {
          currentSession.kitchenCold = reading.waterValue;
        }
        currentSession.kitchenTotal = reading.totalValue.toString();
        currentSession.kitchenDiff = reading.differenceValue.toString();
      } else {
        if (isHot) {
          currentSession.bathroomHot = reading.waterValue;
        } else {
          currentSession.bathroomCold = reading.waterValue;
        }
        currentSession.bathroomTotal = reading.totalValue.toString();
        currentSession.bathroomDiff = reading.differenceValue.toString();
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
      router.push('/login');
      return;
    } else {
      const storedApartment = localStorage.getItem('apartmentNumber');
      if (storedApartment) {
        setApartmentNumber(storedApartment);
      }
    }

    try {
      setLoading(true);
      const meters = await getWaterMetersByUserId(userId);
      const metersWithReadings = await Promise.all(
        meters.map(async (meter: WaterMeterRequest) => {
          const readings = await getMeterReadingByWaterMeterId(meter.id);
          return { ...meter, readings };
        })
      );
      setWaterMeters(metersWithReadings);
    } catch (err: any) {
      console.error('Ошибка при получении данных о счетчиках:', err);
      setError('Не удалось загрузить данные. Пожалуйста, попробуйте позже.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserWaterMeters();
  }, [router, userId]);

  const handleLogout = () => {
    localStorage.removeItem('userId');
    router.push('/login');
  };

  if (!userId) {
    return null;
  }

  return (
    <div className="p-5 max-w-7xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-6 mb-8">
        <div className="flex justify-between items-start mb-6">
          <div className="flex-1">
            <h1 className="text-2xl font-bold text-gray-800 mb-4">Личный кабинет</h1>
          </div>

          {/* Правая часть с кнопкой */}
          <div>
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-500 text-white px-6 py-3 rounded-lg hover:bg-blue-600 transition-colors duration-200 flex items-center"
            >
              <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6" />
              </svg>
              Добавить показания
            </button>
          </div>
        </div>

        {/* Информация о квартире */}
        <div className="bg-blue-50 p-4 rounded-lg mb-6">
          <p className="text-gray-700">г. Магнитогорск</p>
          <p className="text-gray-700">пр-т Ленина, 90</p>
          <p className="font-medium text-gray-800">{apartmentNumber ? `Квартира ${apartmentNumber}` : 'Загрузка...'}</p>
        </div>

        {/* Информация о счетчиках */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <h2 className="text-lg font-semibold text-gray-800 mb-3">Счетчики в ванной</h2>
            <div className="space-y-4">
              {waterMeters.filter(meter => meter.placeOfWaterMeter === 0).map(meter => (
                <div key={meter.id} className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                  <div className="flex items-center mb-2">
                    <div className={`w-3 h-3 rounded-full mr-2 ${meter.waterType === 1 ? 'bg-red-500' : 'bg-blue-500'}`} />
                    <span className="font-medium text-gray-800">
                      {meter.waterType === 1 ? 'Горячая вода' : 'Холодная вода'}
                    </span>
                  </div>
                  <div className="text-sm text-gray-600">
                    <p>Заводской номер: {meter.factoryNumber}</p>
                    <p>Дата установки: {new Date(meter.factoryYear).toLocaleDateString()}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          {waterMeters.some(meter => meter.placeOfWaterMeter === 1) && (
            <div>
              <h2 className="text-lg font-semibold text-gray-800 mb-3">Счетчики на кухне</h2>
              <div className="space-y-4">
                {waterMeters.filter(meter => meter.placeOfWaterMeter === 1).map(meter => (
                  <div key={meter.id} className="bg-gray-50 p-4 rounded-lg border border-gray-200">
                    <div className="flex items-center mb-2">
                      <div className={`w-3 h-3 rounded-full mr-2 ${meter.waterType === 1 ? 'bg-red-500' : 'bg-blue-500'}`} />
                      <span className="font-medium text-gray-800">
                        {meter.waterType === 1 ? 'Горячая вода' : 'Холодная вода'}
                      </span>
                    </div>
                    <div className="text-sm text-gray-600">
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
          userId={userId}
          onSuccess={() => {
            setShowForm(false);
            fetchUserWaterMeters();
          }}
          onCancel={() => setShowForm(false)}
        />
      )}

      {loading ? (
        <div className="mt-4 bg-white rounded-lg shadow-md p-6">
          <p>Загрузка данных...</p>
        </div>
      ) : error ? (
        <div className="mt-4 bg-white rounded-lg shadow-md p-6">
          <p className="text-red-500">{error}</p>
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow-md p-6">
          <h2 className="text-xl font-semibold mb-6">История показаний водомеров</h2>
          {waterMeters.length === 0 ? (
            <p>Пока нет данных о показаниях.</p>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full border-collapse border border-gray-300">
                <thead className="bg-gray-100">
                  <tr>
                    <th className="border border-gray-300 px-4 py-2" rowSpan={2}>Дата</th>
                    <th className="border border-gray-300 px-4 py-2" colSpan={4}>Ванная</th>
                    <th className="border border-gray-300 px-4 py-2" colSpan={4}>Кухня</th>
                  </tr>
                  <tr>
                    <th className="border border-gray-300 px-4 py-2">Горячая вода (м³)</th>
                    <th className="border border-gray-300 px-4 py-2">Холодная вода (м³)</th>
                    <th className="border border-gray-300 px-4 py-2">Сумма показаний</th>
                    <th className="border border-gray-300 px-4 py-2">Потребление</th>
                    <th className="border border-gray-300 px-4 py-2">Горячая вода (м³)</th>
                    <th className="border border-gray-300 px-4 py-2">Холодная вода (м³)</th>
                    <th className="border border-gray-300 px-4 py-2">Сумма показаний</th>
                    <th className="border border-gray-300 px-4 py-2">Потребление</th>
                  </tr>
                </thead>
                <tbody>
                  {combineReadings(waterMeters).map((reading, index) => (
                    <tr key={index} className="text-center">
                      <td className="border border-gray-300 px-4 py-2">
                        {reading.date.toLocaleDateString()}
                      </td>
                      <td className="border border-gray-300 px-4 py-2">{reading.bathroomHot}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.bathroomCold}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.bathroomTotal}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.bathroomDiff}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.kitchenHot}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.kitchenCold}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.kitchenTotal}</td>
                      <td className="border border-gray-300 px-4 py-2">{reading.kitchenDiff}</td>
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