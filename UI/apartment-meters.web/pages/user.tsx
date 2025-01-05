"use client"

// import { useState } from 'react';
// import React, { useEffect } from 'react';
// import UserComponent from '../components/userComponent';
// import { getMeterReadingByUserId } from '@/services/readingMeterService';
// import { useRouter } from 'next/router';

// const UserPage: React.FC = () => {
//     const [userId, setUserId] = useState('');
//     const [password, setPassword] = useState('');
//     const [error, setError] = useState('');
//     const router = useRouter();

//     useEffect(() => {
//         const getMeterReading = async () => {
//             const meterReading = await getMeterReadingByUserId(userId);

//         }
//     })

//     return <UserComponent/>;
// };

// export default UserPage;

import React, { useEffect, useState } from 'react';
import api from '../services/api';
import { useRouter } from 'next/router';
import { getMeterReadingByUserId, MeterReadingRequest } from '../services/readingMeterService';

interface WaterMeterReading {
  date: string;
  hotWater: number;
  coldWater: number;
}

const UserPage: React.FC = () => {
  const [readings, setReadings] = useState<MeterReadingRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const router = useRouter();

  useEffect(() => {
    const fetchUserReadings = async () => {
      const userId = localStorage.getItem('id'); // Получение userId из локального хранилища

      if (!userId) {
        router.push('/login'); // Если userId нет, перенаправляем на логин
        return;
      }

      try {
        const data = await getMeterReadingByUserId(userId);
        setReadings(data);
      } catch (err: any) {
        console.error('Ошибка при получении показаний:', err);
        setError('Не удалось загрузить данные. Пожалуйста, попробуйте позже.');
      } finally {
        setLoading(false);
      }
    };

    fetchUserReadings();
  }, [router]);

  const handleLogout = () => {
    localStorage.removeItem('userId'); // Удаление userId из локального хранилища
    router.push('/login'); // Переход на страницу логина
  };

  return (
    <div style={{ padding: '20px' }}>
      <h1>Добро пожаловать!</h1>
      <button onClick={handleLogout}>Выйти</button>

      {loading ? (
        <p>Загрузка данных...</p>
      ) : error ? (
        <p style={{ color: 'red' }}>{error}</p>
      ) : (
        <div>
          <h2>История показаний водомеров</h2>
          {readings.length === 0 ? (
            <p>Пока нет данных о показаниях.</p>
          ) : (
            <table border={1} cellPadding={10}>
              <thead>
                <tr>
                  <th>Дата</th>
                  <th>Горячая вода (м³)</th>
                  <th>Холодная вода (м³)</th>
                </tr>
              </thead>
              <tbody>
                {readings.map((reading) => (
                  <tr key={reading.id}>
                    <td>{new Date(reading.readingDate).toLocaleDateString()}</td>
                    <td>{reading.hotWaterValue}</td>
                    <td>{reading.coldWaterValue}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      )}
    </div>
  );
};

export default UserPage;