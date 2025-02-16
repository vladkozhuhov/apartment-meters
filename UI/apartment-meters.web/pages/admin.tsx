"use admin"

import React, { useEffect, useState } from 'react';
import { getAllMeterReading } from '../services/readingMeterService';
import { getAllUser } from '../services/userService'; // Добавь импорт
import UsersList from '@/components/UserListComponent';

interface MeterReading {
  id: string;
  userId: string;
  readingDate: string;
  primaryColdWaterValue: string;
  primaryHotWaterValue: string;
  primaryTotalValue: number;
  primaryDifferenceValue: number;
  hasSecondaryMeter: boolean;
  secondaryColdWaterValue: string;
  secondaryHotWaterValue: string;
  secondaryTotalValue: number;
  secondaryDifferenceValue: number;
}

interface Users {
  id: string;
  apartmentNumber: number;
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

  return (
    <div className="p-5">

      <div className="mb-4">
        <label>
          Квартира:
          <input
            type="text"
            name="apartment"
            value={filter.apartment}
            onChange={handleFilterChange}
            className="border px-2 py-1 ml-2"
          />
        </label>
        <label className="ml-4">
          Месяц:
          <select
            name="month"
            value={filter.month}
            onChange={handleFilterChange}
            className="border px-2 py-1 ml-2"
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

        <label className="ml-4">
          Год:
          <select
            name="year"
            value={filter.year}
            onChange={handleFilterChange}
            className="border px-2 py-1 ml-2"
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

        <button 
          onClick={applyFilter} 
          className="ml-4 bg-blue-500 text-white px-4 py-2 rounded"
        >
          Применить фильтр
        </button>

        <button
          onClick={() => setShowForm(true)}
          className="ml-4 bg-blue-600 text-white px-4 py-2 rounded"
        >
          Просмотр пользователей
        </button>

        <button
          // onClick={() => setShowForm(true)}
          className="ml-4 bg-green-600 text-white px-4 py-2 rounded"
        >
          Изменить пользователя
        </button>
      </div>

      {showForm && <UsersList onClose={() => setShowForm(false)} />} {/* Форма пользователей */}

      {loading ? (
        <p>Загрузка данных...</p>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : (
        <table className="w-full border-collapse border border-gray-300 mt-4">
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
              <th className="border border-gray-300 px-4 py-2">Горячая вода (м³) (2 сч.)</th>
              <th className="border border-gray-300 px-4 py-2">Холодная вода (м³) (2 сч.)</th>
              <th className="border border-gray-300 px-4 py-2">Сумма показаний (2 сч.)</th>
              <th className="border border-gray-300 px-4 py-2">Потребление (2 сч.)</th>
            </tr>
          </thead>
          <tbody>
            {filteredReadings.map((reading) => (
              <tr key={reading.id} className="text-center">
                <td className="border border-gray-300 px-4 py-2">
                  {new Date(reading.readingDate).toLocaleDateString("ru-RU", { month: "long" })}
                </td>
                <td className="border border-gray-300 px-4 py-2">
                  {users.find((user) => user.id === reading.userId)?.apartmentNumber || 'Не найден'}
                </td>
                <td className="border border-gray-300 px-4 py-2">{reading.primaryHotWaterValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.primaryColdWaterValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.primaryTotalValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.primaryDifferenceValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.secondaryHotWaterValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.secondaryColdWaterValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.secondaryTotalValue}</td>
                <td className="border border-gray-300 px-4 py-2">{reading.secondaryDifferenceValue}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  );
};

export default AdminPage;
