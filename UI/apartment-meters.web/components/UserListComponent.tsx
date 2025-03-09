import React, { useEffect, useState } from "react";
import { getAllUser, UserRequest, updateUser } from "../services/userService";
import { getWaterMetersByUserId, WaterMeterRequest, WaterMeterUpdateRequest, updateWaterMeter } from "../services/waterMeterService";

interface UsersListProps {
  onClose: () => void;
}

interface UserWithMeters extends UserRequest {
  waterMeters: WaterMeterRequest[];
}

const UsersList: React.FC<UsersListProps> = ({ onClose }) => {
  const [users, setUsers] = useState<UserWithMeters[]>([]);
  const [filteredUsers, setFilteredUsers] = useState<UserWithMeters[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filter, setFilter] = useState({ apartment: '' });
  const [expandedUser, setExpandedUser] = useState<string | null>(null);
  const [editingUser, setEditingUser] = useState<string | null>(null);
  const [editingMeter, setEditingMeter] = useState<string | null>(null);
  const [editForm, setEditForm] = useState<Partial<UserRequest>>({});
  const [editMeterForm, setEditMeterForm] = useState<Partial<WaterMeterRequest>>({});
  const [updateLoading, setUpdateLoading] = useState(false);

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { value } = e.target;
    setFilter({ apartment: value });
    
    if (value) {
      const filtered = users.filter(user => 
        user.apartmentNumber.toString().includes(value)
      );
      setFilteredUsers(filtered);
    } else {
      setFilteredUsers(users);
    }
  };

  const toggleUserExpand = (userId: string) => {
    setExpandedUser(expandedUser === userId ? null : userId);
  };

  const handleEditUser = (user: UserWithMeters) => {
    setEditingUser(user.id);
    setEditForm({
      id: user.id,
      apartmentNumber: user.apartmentNumber,
      lastName: user.lastName,
      firstName: user.firstName,
      middleName: user.middleName,
      phoneNumber: user.phoneNumber,
      password: user.password,
      role: user.role
    });
  };

  const handleEditMeter = (meter: WaterMeterRequest) => {
    setEditingMeter(meter.id);
    setEditMeterForm({
      factoryNumber: meter.factoryNumber,
      factoryYear: meter.factoryYear
    });
  };

  const handleSaveUser = async (userId: string) => {
    try {
      setUpdateLoading(true);
      setError(""); // Очищаем предыдущие ошибки

      const userToUpdate = users.find(u => u.id === userId);
      if (!userToUpdate) {
        setError("Пользователь не найден");
        return;
      }

      // Объединяем существующие данные с изменениями
      const updatedUserData: UserRequest = {
        id: userId,
        apartmentNumber: editForm.apartmentNumber || userToUpdate.apartmentNumber,
        lastName: editForm.lastName || userToUpdate.lastName,
        firstName: editForm.firstName || userToUpdate.firstName,
        middleName: editForm.middleName || userToUpdate.middleName,
        phoneNumber: editForm.phoneNumber || userToUpdate.phoneNumber,
        password: editForm.password || userToUpdate.password,
        role: userToUpdate.role
      };

      console.log('Отправляемые данные:', updatedUserData);
      
      await updateUser(userId, updatedUserData);
      
      // Обновляем данные в локальном состоянии
      const updatedUsers = users.map(user => {
        if (user.id === userId) {
          return {
            ...user,
            ...updatedUserData
          };
        }
        return user;
      });
      
      setUsers(updatedUsers);
      setFilteredUsers(updatedUsers);
      setEditingUser(null);
    } catch (err) {
      console.error('Ошибка при обновлении:', err);
      setError("Ошибка при обновлении данных пользователя. Пожалуйста, проверьте введенные данные.");
    } finally {
      setUpdateLoading(false);
    }
  };

  const handleSaveMeter = async (meterId: string) => {
    try {
      setUpdateLoading(true);
      setError(""); // Очищаем предыдущие ошибки

      const userWithMeter = users.find(user => 
        user.waterMeters.some(meter => meter.id === meterId)
      );
      
      const meterToUpdate = userWithMeter?.waterMeters.find(meter => meter.id === meterId);

      if (!meterToUpdate) {
        setError("Счетчик не найден");
        return;
      }

      // Создаем объект только с измененными полями
      const updatedMeterData: WaterMeterUpdateRequest = {
        id: meterId,
        ...(editMeterForm.factoryNumber !== undefined && { factoryNumber: editMeterForm.factoryNumber }),
        ...(editMeterForm.factoryYear !== undefined && { factoryYear: editMeterForm.factoryYear })
      };

      console.log('Текущий счетчик:', meterToUpdate);
      console.log('Форма редактирования:', editMeterForm);
      console.log('Отправляемые данные для обновления счетчика:', updatedMeterData);
      
      // Отправляем запрос на сервер
      await updateWaterMeter(meterId, updatedMeterData);
      
      // После успешного обновления на сервере, обновляем локальное состояние
      const updatedUsers = users.map(user => ({
        ...user,
        waterMeters: user.waterMeters.map(meter => {
          if (meter.id === meterId) {
            return {
              ...meter,
              ...(editMeterForm.factoryNumber !== undefined && { factoryNumber: editMeterForm.factoryNumber }),
              ...(editMeterForm.factoryYear !== undefined && { factoryYear: editMeterForm.factoryYear })
            };
          }
          return meter;
        })
      }));
      
      setUsers(updatedUsers);
      setFilteredUsers(updatedUsers);
      setEditingMeter(null);
      setEditMeterForm({});
      
      // Сообщаем об успешном обновлении
      console.log('Счетчик успешно обновлен');
    } catch (err) {
      console.error('Ошибка при обновлении счетчика:', err);
      setError("Ошибка при обновлении данных счетчика. Пожалуйста, проверьте введенные данные.");
    } finally {
      setUpdateLoading(false);
    }
  };

  useEffect(() => {
    const fetchUsersWithMeters = async () => {
      try {
        const usersData = await getAllUser();
        
        // Получаем счетчики для каждого пользователя
        const usersWithMeters = await Promise.all(
          usersData.map(async (user: UserRequest) => {
            const waterMeters = await getWaterMetersByUserId(user.id);
            return { ...user, waterMeters };
          })
        );

        setUsers(usersWithMeters);
        setFilteredUsers(usersWithMeters);
      } catch (err) {
        setError("Ошибка загрузки данных пользователей.");
      } finally {
        setLoading(false);
      }
    };

    fetchUsersWithMeters();
  }, []);

  const getMeterTypeText = (type: number) => type === 1 ? "Горячая вода" : "Холодная вода";
  const getMeterLocationText = (location: number) => location === 1 ? "Кухня" : "Ванная";

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center p-4 overflow-y-auto">
      <div className="bg-white rounded-lg shadow-xl w-full max-w-6xl max-h-[90vh] overflow-hidden">
        {/* Заголовок */}
        <div className="px-6 py-4 border-b border-gray-200 flex justify-between items-center bg-gray-50">
          <h2 className="text-xl font-bold text-gray-800">Список пользователей</h2>
          <button 
            onClick={onClose}
            className="text-gray-500 hover:text-gray-700 transition-colors"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Фильтр */}
        <div className="p-6 border-b border-gray-200 bg-blue-50">
          <div className="flex items-center gap-4">
            <div className="flex-1 max-w-xs">
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Поиск по номеру квартиры
              </label>
              <input
                type="text"
                value={filter.apartment}
                onChange={handleFilterChange}
                placeholder="Введите номер квартиры"
                className="w-full rounded-md border border-gray-300 px-3 py-2 focus:border-blue-500 focus:ring-blue-500"
              />
            </div>
          </div>
        </div>

        {/* Содержимое */}
        <div className="overflow-y-auto" style={{ maxHeight: 'calc(90vh - 200px)' }}>
          {loading ? (
            <div className="p-6 text-center">
              <div className="inline-block animate-spin rounded-full h-8 w-8 border-4 border-blue-500 border-t-transparent"></div>
              <p className="mt-2 text-gray-600">Загрузка данных...</p>
            </div>
          ) : error ? (
            <div className="p-6 text-center text-red-500">{error}</div>
          ) : (
            <div className="p-6 space-y-4">
              {filteredUsers.map((user) => (
                <div key={user.id} className="bg-white rounded-lg border border-gray-200 overflow-hidden">
                  {/* Основная информация о пользователе */}
                  <div className="p-4 flex items-center justify-between hover:bg-gray-50">
                    {editingUser === user.id ? (
                      <div className="flex-1 space-y-4">
                        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Номер квартиры</label>
                            <input
                              type="number"
                              value={editForm.apartmentNumber}
                              onChange={(e) => setEditForm(prev => ({ ...prev, apartmentNumber: parseInt(e.target.value) }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Фамилия</label>
                            <input
                              type="text"
                              value={editForm.lastName}
                              onChange={(e) => setEditForm(prev => ({ ...prev, lastName: e.target.value }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Имя</label>
                            <input
                              type="text"
                              value={editForm.firstName}
                              onChange={(e) => setEditForm(prev => ({ ...prev, firstName: e.target.value }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Отчество</label>
                            <input
                              type="text"
                              value={editForm.middleName}
                              onChange={(e) => setEditForm(prev => ({ ...prev, middleName: e.target.value }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Телефон</label>
                            <input
                              type="text"
                              value={editForm.phoneNumber}
                              onChange={(e) => setEditForm(prev => ({ ...prev, phoneNumber: e.target.value }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                          <div>
                            <label className="block text-sm font-medium text-gray-700">Пароль</label>
                            <input
                              type="password"
                              value={editForm.password}
                              onChange={(e) => setEditForm(prev => ({ ...prev, password: e.target.value }))}
                              className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                            />
                          </div>
                        </div>
                        <div className="flex justify-end gap-2">
                          <button
                            onClick={() => setEditingUser(null)}
                            className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200"
                            disabled={updateLoading}
                          >
                            Отмена
                          </button>
                          <button
                            onClick={() => handleSaveUser(user.id)}
                            className="px-4 py-2 text-sm font-medium text-white bg-blue-500 rounded-md hover:bg-blue-600"
                            disabled={updateLoading}
                          >
                            {updateLoading ? 'Сохранение...' : 'Сохранить'}
                          </button>
                        </div>
                      </div>
                    ) : (
                      <>
                        <div className="flex-1">
                          <div className="flex items-center gap-4">
                            <span className="text-lg font-medium">
                              Квартира {user.apartmentNumber}
                            </span>
                            <span className="text-gray-600">
                              {user.lastName} {user.firstName} {user.middleName}
                            </span>
                          </div>
                          <div className="text-sm text-gray-500 mt-1">
                            Телефон: {user.phoneNumber}
                          </div>
                        </div>
                        <div className="flex items-center gap-2">
                          <button
                            onClick={() => handleEditUser(user)}
                            className="p-2 text-blue-600 hover:text-blue-800"
                          >
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                            </svg>
                          </button>
                          <span className={`px-2 py-1 rounded text-sm ${
                            user.role === 1 ? 'bg-purple-100 text-purple-800' : 'bg-green-100 text-green-800'
                          }`}>
                            {user.role === 1 ? 'Админ' : 'Пользователь'}
                          </span>
                          <button
                            onClick={() => toggleUserExpand(user.id)}
                            className="p-2"
                          >
                            <svg 
                              className={`w-5 h-5 transition-transform ${expandedUser === user.id ? 'transform rotate-180' : ''}`}
                              fill="none" 
                              stroke="currentColor" 
                              viewBox="0 0 24 24"
                            >
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 9l-7 7-7-7" />
                            </svg>
                          </button>
                        </div>
                      </>
                    )}
                  </div>

                  {/* Детальная информация и счетчики */}
                  {expandedUser === user.id && (
                    <div className="border-t border-gray-200 p-4 bg-gray-50">
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {user.waterMeters.map((meter) => (
                          <div key={meter.id} className="bg-white p-4 rounded-lg border border-gray-200">
                            {editingMeter === meter.id ? (
                              <div className="space-y-4">
                                <div>
                                  <label className="block text-sm font-medium text-gray-700">Заводской номер</label>
                                  <input
                                    type="number"
                                    value={editMeterForm.factoryNumber}
                                    onChange={(e) => setEditMeterForm(prev => ({ ...prev, factoryNumber: parseInt(e.target.value) }))}
                                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                                  />
                                </div>
                                <div>
                                  <label className="block text-sm font-medium text-gray-700">Дата установки</label>
                                  <input
                                    type="date"
                                    value={new Date(editMeterForm.factoryYear as Date).toISOString().split('T')[0]}
                                    onChange={(e) => setEditMeterForm(prev => ({ ...prev, factoryYear: new Date(e.target.value) }))}
                                    className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2"
                                  />
                                </div>
                                <div className="flex justify-end gap-2">
                                  <button
                                    onClick={() => setEditingMeter(null)}
                                    className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-md hover:bg-gray-200"
                                    disabled={updateLoading}
                                  >
                                    Отмена
                                  </button>
                                  <button
                                    onClick={() => handleSaveMeter(meter.id)}
                                    className="px-4 py-2 text-sm font-medium text-white bg-blue-500 rounded-md hover:bg-blue-600"
                                    disabled={updateLoading}
                                  >
                                    {updateLoading ? 'Сохранение...' : 'Сохранить'}
                                  </button>
                                </div>
                              </div>
                            ) : (
                              <>
                                <div className="flex items-center justify-between mb-2">
                                  <div className="flex items-center gap-2">
                                    <div className={`w-3 h-3 rounded-full ${
                                      meter.waterType === 1 ? 'bg-red-500' : 'bg-blue-500'
                                    }`} />
                                    <h4 className="font-medium">
                                      {getMeterTypeText(meter.waterType)} - {getMeterLocationText(meter.placeOfWaterMeter)}
                                    </h4>
                                  </div>
                                  <button
                                    onClick={() => handleEditMeter(meter)}
                                    className="p-2 text-blue-600 hover:text-blue-800"
                                  >
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
                                    </svg>
                                  </button>
                                </div>
                                <div className="space-y-1 text-sm text-gray-600">
                                  <p>Заводской номер: {meter.factoryNumber}</p>
                                  <p>Дата установки: {new Date(meter.factoryYear).toLocaleDateString()}</p>
                                </div>
                              </>
                            )}
                          </div>
                        ))}
                      </div>
                    </div>
                  )}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default UsersList;