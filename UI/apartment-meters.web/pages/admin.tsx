"use client"

import React, { useEffect, useState } from 'react';
import { getAllMeterReading } from '../services/readingMeterService';
import { getAllUser, getPaginatedUsersWithMeters, UserRequest, PaginatedUsersResponse, UserWithMetersAndReadings } from '../services/userService';
import UsersList from '@/components/UserListComponent';
import { getWaterMetersByUserId } from '../services/waterMeterService';
import { isAuthenticated, logout } from '../services/authService';
import { useRouter } from 'next/router';
import EditMeterReadingComponent from '@/components/EditMeterReadingComponent';
import AdminAddMeterReadingComponent from '@/components/AdminAddMeterReadingComponent';

interface MeterReading {
  id: string;
  waterValue: string;
  differenceValue: number;
  readingDate: Date;
  waterMeterId: string;
  placeOfWaterMeter?: number;
  waterType?: number;
}

interface WaterMeter {
  id: string;
  userId: string;
  placeOfWaterMeter: number;
  waterType: number;
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
  bathroomHotDiff: string;
  bathroomCold: string;
  bathroomColdDiff: string;
  kitchenHot: string;
  kitchenHotDiff: string;
  kitchenCold: string;
  kitchenColdDiff: string;
}

interface UsersListProps {
  onClose: () => void;
}

// Типы для сортировки
type SortColumn = 'date' | 'apartment';
type SortDirection = 'asc' | 'desc';

// Функция для отображения только месяца в колонке "Дата"
const formatDate = (date: Date): string => {
  return new Date(date).toLocaleDateString('ru-RU', { month: 'long' }).replace(/^./, str => str.toUpperCase());
};

const AdminPage: React.FC = () => {
  const [readings, setReadings] = useState<MeterReading[]>([]);
  const [filteredReadings, setFilteredReadings] = useState<MeterReading[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [filter, setFilter] = useState({ apartment: '', month: '', year: '' });  
  const [showForm, setShowForm] = useState(false); // Управление формой пользователей
  const [users, setUsers] = useState<Users[]>([]);
  const [waterMeters, setWaterMeters] = useState<WaterMeter[]>([]);
  const [isClient, setIsClient] = useState(false);
  const [combinedReadings, setCombinedReadings] = useState<CombinedReading[]>([]);
  const router = useRouter();
  
  // Состояние для сортировки
  const [sortConfig, setSortConfig] = useState<{
    column: SortColumn;
    direction: SortDirection;
  } | null>(null);
  
  // Добавляем состояние для пагинации
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(20);
  const [totalPages, setTotalPages] = useState(1);
  const [totalUsers, setTotalUsers] = useState(0);
  const [readingsPerPage, setReadingsPerPage] = useState(20);
  const [currentReadingsPage, setCurrentReadingsPage] = useState(1);
  
  // Состояние для отслеживания редактируемого показания
  const [editingReadingId, setEditingReadingId] = useState<string | null>(null);
  
  // Состояние для отображения формы добавления показаний
  const [showAddReadingsForm, setShowAddReadingsForm] = useState(false);
  
  // Устанавливаем флаг клиентского рендеринга
  useEffect(() => {
    setIsClient(true);
  }, []);
  
  // Проверяем аутентификацию при загрузке страницы
  useEffect(() => {
    // Проверяем, что мы на клиенте
    if (!isClient) return;
    
    console.log('Проверка аутентификации на странице администратора');
    const authStatus = isAuthenticated();
    console.log('Результат проверки аутентификации:', authStatus);
    
    if (!authStatus) {
      console.log('Перенаправление на страницу входа из-за отсутствия аутентификации');
      router.push('/login');
      return;
    }
    
    // Проверка на наличие роли администратора может быть добавлена здесь
  }, [router, isClient]);
  
  // Обновленный метод для получения данных с пагинацией
  const fetchPaginatedData = async () => {
    try {
      setLoading(true);
      // Получаем данные с сервера с пагинацией
      const data: PaginatedUsersResponse = await getPaginatedUsersWithMeters(page, pageSize);
      console.log('Получены пагинированные данные:', data);
      
      // Обновляем метаданные пагинации
      setTotalPages(data.totalPages);
      setTotalUsers(data.totalCount);
      
      // Извлекаем пользователей
      const usersList = data.items.map(user => ({
        id: user.id,
        apartmentNumber: user.apartmentNumber
      }));
      setUsers(usersList);
      
      // Извлекаем водомеры
      const waterMetersList: WaterMeter[] = [];
      
      // Извлекаем показания
      const allReadings: MeterReading[] = [];
      
      // Обрабатываем полученные данные
      data.items.forEach(user => {
        user.waterMeters.forEach(waterMeter => {
          // Добавляем водомер
          waterMetersList.push({
            id: waterMeter.id,
            userId: waterMeter.userId,
            placeOfWaterMeter: waterMeter.placeOfWaterMeter,
            waterType: waterMeter.waterType,
            factoryNumber: parseInt(waterMeter.factoryNumber),
            factoryYear: new Date(waterMeter.factoryYear)
          });
          
          // Добавляем показания
          waterMeter.readings.forEach(reading => {
            allReadings.push({
              id: reading.id,
              waterMeterId: reading.waterMeterId,
              waterValue: reading.waterValue,
              differenceValue: reading.differenceValue,
              readingDate: new Date(reading.readingDate),
              placeOfWaterMeter: waterMeter.placeOfWaterMeter,
              waterType: waterMeter.waterType
            });
          });
        });
      });
      
      // Обновляем состояние
      setWaterMeters(waterMetersList);
      setReadings(allReadings);
      setFilteredReadings(allReadings);
    } 
    catch (err) {
      console.error('Ошибка при загрузке данных:', err);
      setError('Не удалось загрузить данные. Попробуйте позже.');
    } 
    finally {
      setLoading(false);
    }
  };

  // Используем обновленный метод загрузки данных
  useEffect(() => {
    // Проверяем, что мы на клиенте
    if (!isClient) return;
    
    fetchPaginatedData();
  }, [isClient, page, pageSize]);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilter((prev) => ({ ...prev, [name]: value }));
  };

  const handleUpdate = async (id: string, updatedData: Partial<MeterReading>) => {
    try {
    //   await updateUser(id, updatedData);
      fetchPaginatedData(); // Обновляем данные после редактирования
    } catch (err) {
      console.error('Ошибка при обновлении данных:', err);
      setError('Не удалось обновить данные.');
    }
  };

  const getUserIdByWaterMeterId = (waterMeterId: string): string | undefined => {
    const waterMeter = waterMeters.find(wm => wm.id === waterMeterId);
    return waterMeter?.userId;
  };

  const combineReadings = (readings: MeterReading[]): CombinedReading[] => {
    const allReadings: CombinedReading[] = [];
    
    // Группируем по уникальной дате и идентификатору пользователя
    const readingMap = new Map<string, MeterReading[]>();
    
    // Группировка показаний по дате (только дата без времени) и userId
    readings.forEach((reading) => {
      const waterMeter = waterMeters.find(wm => wm.id === reading.waterMeterId);
      if (!waterMeter) return;
      
      const userId = waterMeter.userId;
      const readingDate = new Date(reading.readingDate);
      // Создаем ключ на основе даты (без времени) и ID пользователя
      const dateString = `${readingDate.getFullYear()}-${readingDate.getMonth()}-${readingDate.getDate()}`;
      const key = `${dateString}_${userId}`;
      
      if (!readingMap.has(key)) {
        readingMap.set(key, []);
      }
      
      readingMap.get(key)!.push(reading);
    });
    
    // Обрабатываем каждую группу показаний
    readingMap.forEach((groupReadings, key) => {
      // Находим первое показание для получения даты группы
      const firstReading = groupReadings[0];
      const firstWaterMeter = waterMeters.find(wm => wm.id === firstReading.waterMeterId);
      if (!firstWaterMeter) return;
      
      const session: CombinedReading = {
        date: new Date(firstReading.readingDate),
        userId: firstWaterMeter.userId,
        bathroomHot: '-',
        bathroomHotDiff: '-',
        bathroomCold: '-',
        bathroomColdDiff: '-',
        kitchenHot: '-',
        kitchenHotDiff: '-',
        kitchenCold: '-',
        kitchenColdDiff: '-',
      };
      
      // Заполняем данные по каждому показанию в группе
      groupReadings.forEach(reading => {
        const waterMeter = waterMeters.find(wm => wm.id === reading.waterMeterId);
        if (!waterMeter) return;
        
        const isKitchen = waterMeter.placeOfWaterMeter === 1;
        const isHot = waterMeter.waterType === 1;
        
        if (isKitchen) {
          if (isHot) {
            session.kitchenHot = reading.waterValue;
            session.kitchenHotDiff = reading.differenceValue.toFixed(3);
          } else {
            session.kitchenCold = reading.waterValue;
            session.kitchenColdDiff = reading.differenceValue.toFixed(3);
          }
        } else {
          if (isHot) {
            session.bathroomHot = reading.waterValue;
            session.bathroomHotDiff = reading.differenceValue.toFixed(3);
          } else {
            session.bathroomCold = reading.waterValue;
            session.bathroomColdDiff = reading.differenceValue.toFixed(3);
          }
        }
      });
      
      allReadings.push(session);
    });
    
    console.log('Сгруппированные показания:', allReadings);
    
    // Сортируем сгруппированные данные в соответствии с текущими настройками сортировки
    return sortData(allReadings);
  };

  // Функция для сортировки данных
  const sortData = (data: CombinedReading[]): CombinedReading[] => {
    // Если сортировка не задана, возвращаем исходные данные
    if (!sortConfig) {
      return data.sort((a, b) => a.date.getTime() - b.date.getTime()); // По умолчанию по дате (старые записи сверху)
    }

    return [...data].sort((a, b) => {
      // Для колонки "Квартира" нужно найти соответствующие номера квартир
      if (sortConfig.column === 'apartment') {
        const userA = users.find(user => user.id === a.userId);
        const userB = users.find(user => user.id === b.userId);
        const aptA = userA ? userA.apartmentNumber : 0;
        const aptB = userB ? userB.apartmentNumber : 0;
        
        if (sortConfig.direction === 'asc') {
          return aptA - aptB;
        } else {
          return aptB - aptA;
        }
      }
      
      // Для даты
      if (sortConfig.column === 'date') {
        if (sortConfig.direction === 'asc') {
          return a.date.getTime() - b.date.getTime();
        } else {
          return b.date.getTime() - a.date.getTime();
        }
      }
      
      return 0; // По умолчанию не меняем порядок
    });
  };

  // Обработчик для изменения сортировки при клике на заголовок таблицы
  const handleSort = (column: SortColumn) => {
    let direction: SortDirection = 'asc';
    
    // Если уже сортировали по этой колонке, меняем направление сортировки
    if (sortConfig && sortConfig.column === column && sortConfig.direction === 'asc') {
      direction = 'desc';
    }
    
    setSortConfig({ column, direction });
  };

  // Функция для отображения иконки сортировки
  const renderSortIcon = (column: SortColumn) => {
    if (!sortConfig || sortConfig.column !== column) {
      return <span className="ml-1 text-gray-400">⇅</span>;
    }
    
    return sortConfig.direction === 'asc' 
      ? <span className="ml-1 text-blue-500">↑</span> 
      : <span className="ml-1 text-blue-500">↓</span>;
  };

  // Add pagination controls
  const handlePrevPage = () => {
    if (page > 1) {
      setPage(prev => prev - 1);
    }
  };
  
  const handleNextPage = () => {
    if (page < totalPages) {
      setPage(prev => prev + 1);
    }
  };
  
  const handlePageSizeChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
    const newSize = parseInt(e.target.value);
    setPageSize(newSize);
    setPage(1); // Reset to first page when changing page size
  };

  // Обработчик для фильтрации и пагинации показаний
  useEffect(() => {
    if (readings.length > 0) {
      let result = readings;

      // Применяем фильтры
      if (filter.apartment) {
        // Находим пользователя по номеру квартиры
        const filteredUsers = users.filter(
          user => user.apartmentNumber.toString() === filter.apartment
        );
        
        // Находим все водомеры этих пользователей
        const userIds = filteredUsers.map(user => user.id);
        const relevantWaterMeterIds = waterMeters
          .filter(wm => userIds.includes(wm.userId))
          .map(wm => wm.id);
        
        // Фильтруем показания по найденным водомерам
        result = result.filter(
          reading => relevantWaterMeterIds.includes(reading.waterMeterId)
        );
      }

      if (filter.month) {
        result = result.filter(
          (r) => new Date(r.readingDate).getMonth() + 1 === parseInt(filter.month)
        );
      }

      if (filter.year) {
        result = result.filter(
          (r) => new Date(r.readingDate).getFullYear() === parseInt(filter.year)
        );
      }

      setFilteredReadings(result);
      
      // Группируем и комбинируем показания
      const combined = combineReadings(result);
      setCombinedReadings(combined);
    }
  }, [readings, filter, waterMeters, users, sortConfig]);

  // Функция для начала редактирования показания
  const handleEditReading = (readingId: string) => {
    setEditingReadingId(readingId);
  };

  // Функция для завершения редактирования
  const handleEditSuccess = () => {
    // Закрываем модальное окно
    setEditingReadingId(null);
    
    // Обновляем данные с небольшой задержкой, чтобы сервер успел обработать изменения
    setTimeout(() => {
      fetchPaginatedData(); // Обновляем данные после редактирования
    }, 500);
  };

  // Функция для отмены редактирования
  const handleEditCancel = () => {
    setEditingReadingId(null);
  };

  // Если страница еще не загружена на клиенте, показываем пустую разметку
  if (!isClient) {
    return <div className="min-h-screen flex items-center justify-center"></div>;
  }

  return (
    <div className="p-2 sm:p-5 w-full max-w-full sm:max-w-7xl mx-auto">
      <div className="bg-white rounded-lg shadow-md p-4 sm:p-6 mb-6 sm:mb-8">
        <div className="flex justify-between items-start mb-4 sm:mb-6">
          <div className="flex-1">
            <h1 className="text-xl sm:text-2xl font-bold text-gray-800 mb-4">Панель администратора</h1>
          </div>
        </div>

        {/* Фильтры */}
        <div className="bg-blue-50 p-3 sm:p-4 rounded-lg mb-4 sm:mb-6">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-3 sm:gap-4 mb-3 sm:mb-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Квартира:
                <input
                  type="text"
                  name="apartment"
                  value={filter.apartment}
                  onChange={handleFilterChange}
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500 text-sm"
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
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500 text-sm"
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
                  className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500 text-sm"
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
          </div>
          
          {/* Кнопки в одну строку */}
          <div className="flex flex-wrap gap-2 justify-end">
            <button
              onClick={() => setShowForm(true)}
              className="bg-blue-600 text-white px-5 py-2.5 rounded-lg hover:bg-blue-700 transition-colors duration-200 text-sm font-medium"
            >
              Просмотр пользователей
            </button>
            <button
              onClick={() => setShowAddReadingsForm(true)}
              className="bg-green-600 text-white px-5 py-2.5 rounded-lg hover:bg-green-700 transition-colors duration-200 text-sm font-medium"
            >
              Добавить показания
            </button>
          </div>
        </div>

        {showForm && <UsersList onClose={() => setShowForm(false)} />}

        {showAddReadingsForm && (
          <AdminAddMeterReadingComponent
            onSuccess={() => {
              setShowAddReadingsForm(false);
              fetchPaginatedData(); // Обновляем данные после добавления
            }}
            onCancel={() => setShowAddReadingsForm(false)}
          />
        )}

        {loading ? (
          <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
            <p>Загрузка данных...</p>
          </div>
        ) : error ? (
          <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
            <p className="text-red-500">{error}</p>
          </div>
        ) : (
          <div className="bg-white rounded-lg shadow-md p-4 sm:p-6">
            <h2 className="text-lg sm:text-xl font-semibold mb-4 sm:mb-6">История показаний водомеров</h2>
            
            {/* Pagination Controls for Users */}
            <div className="flex justify-between items-center mb-4">
              <div className="text-sm text-gray-600">
                Всего: {totalUsers} пользователей, {readings.length} показаний ({combinedReadings.length} записей)
              </div>
              <div className="flex items-center space-x-2">
                <label className="text-sm text-gray-600">
                  Пользователей на странице:
                  <select 
                    value={pageSize} 
                    onChange={handlePageSizeChange}
                    className="ml-2 border rounded p-1"
                  >
                    <option value={10}>10</option>
                    <option value={20}>20</option>
                    <option value={50}>50</option>
                    <option value={100}>100</option>
                  </select>
                </label>
                <button 
                  onClick={handlePrevPage} 
                  disabled={page === 1}
                  className={`px-3 py-1 rounded ${page === 1 ? 'bg-gray-200 text-gray-500' : 'bg-blue-500 text-white hover:bg-blue-600'}`}
                >
                  &lt; Назад
                </button>
                <span className="text-sm">
                  Страница {page} из {totalPages}
                </span>
                <button 
                  onClick={handleNextPage} 
                  disabled={page === totalPages}
                  className={`px-3 py-1 rounded ${page === totalPages ? 'bg-gray-200 text-gray-500' : 'bg-blue-500 text-white hover:bg-blue-600'}`}
                >
                  Вперед &gt;
                </button>
              </div>
            </div>
            
            <div className="-mx-4 sm:mx-0 overflow-x-auto">
              <table className="w-full border-collapse border border-gray-300 text-xs sm:text-sm">
                <thead className="bg-gray-100">
                  <tr>
                    <th className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2" rowSpan={2}>
                      <button 
                        className="w-full text-left font-medium flex items-center justify-between text-xs sm:text-sm" 
                        onClick={() => handleSort('date')}
                      >
                        Дата {renderSortIcon('date')}
                      </button>
                    </th>
                    <th className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2" rowSpan={2}>
                      <button 
                        className="w-full text-left font-medium flex items-center justify-between text-xs sm:text-sm" 
                        onClick={() => handleSort('apartment')}
                      >
                        Квартира {renderSortIcon('apartment')}
                      </button>
                    </th>
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
                  {/* Отображаем все показания без пагинации */}
                  {combineReadings(filteredReadings).map((reading, index) => {
                    const user = users.find((user) => user.id === reading.userId);
                    return (
                      <tr key={index} className="text-center hover:bg-gray-50">
                        <td className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2">
                          {formatDate(reading.date)}
                        </td>
                        <td className="border border-gray-300 px-2 py-1 sm:px-4 sm:py-2">
                          {user ? user.apartmentNumber : 'Не найден'}
                        </td>
                        
                        {/* Ванная - горячая вода */}
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2 relative group">
                          {reading.bathroomHot}
                          {reading.bathroomHot !== '-' && (
                            <button
                              onClick={() => {
                                // Найти показание по водомеру
                                const bathHotMeter = waterMeters.find(meter => 
                                  meter.placeOfWaterMeter === 0 && meter.waterType === 1 && meter.userId === reading.userId
                                );
                                if (bathHotMeter) {
                                  // Найти показание по этому водомеру и этой дате
                                  const readingData = readings.find(r => 
                                    r.waterMeterId === bathHotMeter.id && 
                                    new Date(r.readingDate).toDateString() === reading.date.toDateString()
                                  );
                                  if (readingData) {
                                    handleEditReading(readingData.id);
                                  }
                                }
                              }}
                              className="opacity-0 group-hover:opacity-100 absolute top-1 right-1 p-1 bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200"
                              title="Редактировать показание"
                            >
                              <svg className="w-3 h-3" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                              </svg>
                            </button>
                          )}
                        </td>
                        
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">
                          {reading.bathroomHotDiff}
                        </td>
                        
                        {/* Ванная - холодная вода */}
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2 relative group">
                          {reading.bathroomCold}
                          {reading.bathroomCold !== '-' && (
                            <button
                              onClick={() => {
                                const bathColdMeter = waterMeters.find(meter => 
                                  meter.placeOfWaterMeter === 0 && meter.waterType === 0 && meter.userId === reading.userId
                                );
                                if (bathColdMeter) {
                                  const readingData = readings.find(r => 
                                    r.waterMeterId === bathColdMeter.id && 
                                    new Date(r.readingDate).toDateString() === reading.date.toDateString()
                                  );
                                  if (readingData) {
                                    handleEditReading(readingData.id);
                                  }
                                }
                              }}
                              className="opacity-0 group-hover:opacity-100 absolute top-1 right-1 p-1 bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200"
                              title="Редактировать показание"
                            >
                              <svg className="w-3 h-3" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                              </svg>
                            </button>
                          )}
                        </td>
                        
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">
                          {reading.bathroomColdDiff}
                        </td>
                        
                        {/* Кухня - горячая вода */}
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2 relative group">
                          {reading.kitchenHot}
                          {reading.kitchenHot !== '-' && (
                            <button
                              onClick={() => {
                                const kitchenHotMeter = waterMeters.find(meter => 
                                  meter.placeOfWaterMeter === 1 && meter.waterType === 1 && meter.userId === reading.userId
                                );
                                if (kitchenHotMeter) {
                                  const readingData = readings.find(r => 
                                    r.waterMeterId === kitchenHotMeter.id && 
                                    new Date(r.readingDate).toDateString() === reading.date.toDateString()
                                  );
                                  if (readingData) {
                                    handleEditReading(readingData.id);
                                  }
                                }
                              }}
                              className="opacity-0 group-hover:opacity-100 absolute top-1 right-1 p-1 bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200"
                              title="Редактировать показание"
                            >
                              <svg className="w-3 h-3" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                              </svg>
                            </button>
                          )}
                        </td>
                        
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">
                          {reading.kitchenHotDiff}
                        </td>
                        
                        {/* Кухня - холодная вода */}
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2 relative group">
                          {reading.kitchenCold}
                          {reading.kitchenCold !== '-' && (
                            <button
                              onClick={() => {
                                const kitchenColdMeter = waterMeters.find(meter => 
                                  meter.placeOfWaterMeter === 1 && meter.waterType === 0 && meter.userId === reading.userId
                                );
                                if (kitchenColdMeter) {
                                  const readingData = readings.find(r => 
                                    r.waterMeterId === kitchenColdMeter.id && 
                                    new Date(r.readingDate).toDateString() === reading.date.toDateString()
                                  );
                                  if (readingData) {
                                    handleEditReading(readingData.id);
                                  }
                                }
                              }}
                              className="opacity-0 group-hover:opacity-100 absolute top-1 right-1 p-1 bg-blue-100 text-blue-700 rounded-full hover:bg-blue-200"
                              title="Редактировать показание"
                            >
                              <svg className="w-3 h-3" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                                <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                              </svg>
                            </button>
                          )}
                        </td>
                        
                        <td className="border border-gray-300 px-1 py-1 sm:px-4 sm:py-2">
                          {reading.kitchenColdDiff}
                        </td>
                      </tr>
                    );
                  })}
                </tbody>
              </table>
            </div>
          </div>
        )}
      </div>
      
      {/* Модальное окно редактирования показания */}
      {editingReadingId && (
        <EditMeterReadingComponent
          readingId={editingReadingId}
          onSuccess={handleEditSuccess}
          onCancel={handleEditCancel}
        />
      )}
    </div>
  );
};

export default AdminPage;
