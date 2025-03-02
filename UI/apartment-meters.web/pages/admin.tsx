"use admin"

import React, { useEffect, useState } from 'react';
import { getAllMeterReading } from '../services/readingMeterService';
import { getAllUser } from '../services/userService'; // Добавь импорт
import UsersList from '@/components/UserListComponent';

interface MeterReading {
  id: string;
  waterValue: string;
  totalValue: number;
  differenceValue: number;
  readingDate: Date;
  waterMeterId: string; // ID счетчика воды
  userId: string; // ID пользователя
}

interface WaterMeter {
  id: string;
  userId: string;
  placeOfWaterMeter: number; // 0 - ванная, 1 - кухня
  waterType: number; // 0 - холодная, 1 - горячая
  factoryNumber: number;
  factoryYear: Date;
}

interface Users {
  id: string;
  apartmentNumber: number;
}

interface CombinedReading {
  date: Date;
  userId: string;
  bathroomHot: string;
  bathroomCold: string;
  bathroomTotal: string;
  bathroomDiff: string;
  kitchenHot: string;
  kitchenCold: string;
  kitchenTotal: string;
  kitchenDiff: string;
}

interface UsersListProps {
  onClose: () => void;
}

const AdminPage: React.FC = () => {
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [filteredReadings, setFilteredReadings] = useState<MeterReading[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filter, setFilter] = useState({ apartment: '', month: '', year: '' });  
  const [showForm, setShowForm] = useState(false); // Управление формой пользователей
  const [users, setUsers] = useState<Users[]>([]);
  
  const fetchReadings = async () => {
    try {
      setLoading(true);
      const data = await getAllMeterReading();
      console.log('Полученные показания:', JSON.stringify(data, null, 2));
      setReadings(data);
      setFilteredReadings(data);
    } 
    catch (err) {
      console.error('Ошибка при загрузке данных:', err);
      setError('Не удалось загрузить данные. Попробуйте позже.');
    } 
    finally {
      setLoading(false);
    }
  };

  const fetchUsers = async () => {
    try {
      const usersData = await getAllUser();
      console.log('Полученные пользователи:', JSON.stringify(usersData, null, 2));
      setUsers(usersData);
    } 
    catch (err) {
      console.error('Ошибка при загрузке пользователей:', err);
    }
  };

  useEffect(() => {
    fetchReadings();
    fetchUsers();
  }, []);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilter((prev) => ({ ...prev, [name]: value }));
  };

  const applyFilter = () => {
    let result = readings;

    if (filter.apartment) {
      result = result.filter(
        (r) => r.userId === filter.apartment);
    }
    if (filter.month) {
      result = result.filter(
        (r) => new Date(r.readingDate).getMonth() + 1 === parseInt(filter.month));
    }
    if (filter.year) {
      result = result.filter(
        (r) => new Date(r.readingDate).getFullYear() === parseInt(filter.year)
      );
    }

    setFilteredReadings(result);
  };

  const handleUpdate = async (id: string, updatedData: Partial<MeterReading>) => {
    try {
    //   await updateUser(id, updatedData);
      fetchReadings(); // Обновляем данные после редактирования
    } catch (err) {
      console.error('Ошибка при обновлении данных:', err);
      setError('Не удалось обновить данные.');
    }
  };

  const combineReadings = (readings: MeterReading[]): CombinedReading[] => {
    const allReadings: CombinedReading[] = [];
    
    // Сначала сгруппируем все показания по сессиям
    const allMeterReadings = [...readings];

    // Сортируем все показания по времени
    allMeterReadings.sort((a, b) => {
      const timeA = new Date(a.readingDate).getTime();
      const timeB = new Date(b.readingDate).getTime();
      return timeA - timeB;
    });

    // Группируем показания в сессии
    let currentSession: CombinedReading | null = null;

    allMeterReadings.forEach((reading) => {
      const readingDate = new Date(reading.readingDate);
      console.log('Обработка показания:', { 
        readingDate, 
        userId: reading.userId,
        waterMeterId: reading.waterMeterId,
        waterValue: reading.waterValue 
      });

      // Если это первое показание или прошло больше минуты с предыдущего,
      // создаем новую сессию
      if (!currentSession || 
          Math.abs(readingDate.getTime() - new Date(currentSession.date).getTime()) > 60000) {
        if (currentSession) {
          allReadings.push(currentSession);
        }
        currentSession = {
          date: readingDate,
          userId: reading.userId,
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
      // Здесь нужно будет изменить логику после того, как увидим структуру данных в консоли
      // ... оставляем остальной код без изменений ...
    });

    // Добавляем последнюю сессию
    if (currentSession) {
      allReadings.push(currentSession);
    }

    return allReadings.sort((a, b) => a.date.getTime() - b.date.getTime());
  };

  return (
    <div className="p-5 max-w-7xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-6 mb-8">
        <div className="flex justify-between items-start mb-6">
          <div className="flex-1">
            <h1 className="text-2xl font-bold text-gray-800 mb-4">Панель администратора</h1>
          </div>
        </div>

        {/* Фильтры */}
        <div className="bg-blue-50 p-4 rounded-lg mb-6">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Квартира:
                <input
                  type="text"
                  name="apartment"
                  value={filter.apartment}
                  onChange={handleFilterChange}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500"
                />
              </label>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Месяц:
                <select
                  name="month"
                  value={filter.month}
                  onChange={handleFilterChange}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500"
                >
                  <option value="">Все</option>
                  <option value="1">Январь</option>
                  <option value="2">Февраль</option>
                  <option value="3">Март</option>
                  <option value="4">Апрель</option>
                  <option value="5">Май</option>
                  <option value="6">Июнь</option>
                  <option value="7">Июль</option>
                  <option value="8">Август</option>
                  <option value="9">Сентябрь</option>
                  <option value="10">Октябрь</option>
                  <option value="11">Ноябрь</option>
                  <option value="12">Декабрь</option>
                </select>
              </label>
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Год:
                <select
                  name="year"
                  value={filter.year}
                  onChange={handleFilterChange}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500"
                >
                  <option value="">Все</option>
                  {[...Array(10)].map((_, i) => {
                    const year = new Date().getFullYear() - i;
                    return (
                      <option key={year} value={year}>
                        {year}
                      </option>
                    );
                  })}
                </select>
              </label>
            </div>
            <div className="flex items-end space-x-2">
              <button 
                onClick={applyFilter} 
                className="bg-blue-500 text-white px-4 py-2 rounded-lg hover:bg-blue-600 transition-colors duration-200"
              >
                Применить фильтр
              </button>
              <button
                onClick={() => setShowForm(true)}
                className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors duration-200"
              >
                Просмотр пользователей
              </button>
            </div>
          </div>
        </div>

        {showForm && <UsersList onClose={() => setShowForm(false)} />}

        {loading ? (
          <div className="bg-white rounded-lg shadow-md p-6">
            <p>Загрузка данных...</p>
          </div>
        ) : error ? (
          <div className="bg-white rounded-lg shadow-md p-6">
            <p className="text-red-500">{error}</p>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow-md p-6">
            <h2 className="text-xl font-semibold mb-6">История показаний водомеров</h2>
            <div className="overflow-x-auto">
              <table className="w-full border-collapse border border-gray-300">
                <thead className="bg-gray-100">
                  <tr>
                    <th className="border border-gray-300 px-4 py-2" rowSpan={2}>Дата</th>
                    <th className="border border-gray-300 px-4 py-2" rowSpan={2}>Квартира</th>
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
                  {combineReadings(filteredReadings).map((reading, index) => {
                    const user = users.find((user) => user.id === reading.userId);
                    console.log('Поиск пользователя:', { userId: reading.userId, foundUser: user });
                    return (
                      <tr key={index} className="text-center hover:bg-gray-50">
                        <td className="border border-gray-300 px-4 py-2">
                          {new Date(reading.date).toLocaleDateString('ru-RU', { month: 'long' }).replace(/^./, str => str.toUpperCase())}
                        </td>
                        <td className="border border-gray-300 px-4 py-2">
                          {user ? user.apartmentNumber : 'Не найден'}
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
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default AdminPage;
