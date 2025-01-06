"use client"

import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { useRouter } from 'next/router';
import { getMeterReadingByUserId, MeterReadingRequest } from '../services/readingMeterService';
import AddMeterReadingForm from '@/components/addMeterReadingFormComponent';

interface WaterMeterReading {
  date: string;
  hotWater: number;
  coldWater: number;
}

const UserPage: React.FC = () => {
  const [readings, setReadings] = useState<MeterReadingRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [showForm, setShowForm] = useState(false);
  const router = useRouter();

  const userId = localStorage.getItem('id'); // Получение userId из локального хранилища

  const fetchUserReadings = async () => {
    if (!userId) {
      router.push('/login'); // Если userId нет, перенаправляем на логин
      return;
    }

    try {
      setLoading(true);
      const data = await getMeterReadingByUserId(userId);
      setReadings(data);
    } catch (err: any) {
      console.error('Ошибка при получении показаний:', err);
      setError('Не удалось загрузить данные. Пожалуйста, попробуйте позже.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUserReadings();
  }, [router, userId]);

  const handleLogout = () => {
    localStorage.removeItem('userId'); // Удаление userId из локального хранилища
    router.push('/login'); // Переход на страницу логина
  };

  if (!userId) {
    return null; // Пока перенаправление не произошло, не рендерим компонент
  }

  return (
    <div className="p-5">
      <h1 className="text-2xl font-bold mb-4">Добро пожаловать!</h1>
      <button
        onClick={handleLogout}
        className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
      >
        Выйти
      </button>

      {loading ? (
        <p className="mt-4">Загрузка данных...</p>
      ) : error ? (
        <p className="mt-4 text-red-500">{error}</p>
      ) : (
        <div>
          <h2 className="text-xl font-semibold mt-6">История показаний водомеров</h2>
          {readings.length === 0 ? (
            <p className="mt-4">Пока нет данных о показаниях.</p>
          ) : (
            <table className="w-full mt-4 border-collapse border border-gray-300">
              <thead className="bg-gray-100">
                <tr>
                  <th className="border border-gray-300 px-4 py-2">Дата</th>
                  <th className="border border-gray-300 px-4 py-2">Горячая вода (м³)</th>
                  <th className="border border-gray-300 px-4 py-2">Холодная вода (м³)</th>
                </tr>
              </thead>
              <tbody>
                {readings.map((reading) => (
                  <tr key={reading.id} className="text-center">
                    <td className="border border-gray-300 px-4 py-2">
                      {new Date(reading.readingDate).toLocaleDateString()}
                    </td>
                    <td className="border border-gray-300 px-4 py-2">{reading.hotWaterValue}</td>
                    <td className="border border-gray-300 px-4 py-2">{reading.coldWaterValue}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}

          <button
            onClick={() => setShowForm(true)}
            className="mt-6 bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
          >
            Добавить показания
          </button>

          {showForm && (
            <AddMeterReadingForm
              userId={userId}
              onSuccess={() => {
                setShowForm(false);
                fetchUserReadings(); // Обновляем данные после успешного добавления
              }}
              onCancel={() => setShowForm(false)}
            />
          )}
        </div>
      )}
    </div>
  );
};

export default UserPage;