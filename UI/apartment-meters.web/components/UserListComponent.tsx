import React, { useEffect, useState } from "react";
import { getAllUser, UserRequest, updateUser } from "../services/userService";
import { getWaterMetersByUserId, WaterMeterRequest, WaterMeterUpdateRequest, updateWaterMeter } from "../services/waterMeterService";
import { useError } from '../contexts/ErrorContext';
import { ErrorType } from '../hooks/useErrorHandler';

interface UsersListProps {
  onClose: () => void;
}

interface UserWithMeters extends UserRequest {
  waterMeters: WaterMeterRequest[];
}

// Группы кодов ошибок для удобства обработки
const USER_ERRORS = {
  // Ошибки номера квартиры
  APARTMENT_NUMBER: [
    ErrorType.EmptyApartmentNumberError450,
    ErrorType.InvalidApartmentNumberError451
  ],
  // Ошибки ФИО
  NAME: [
    ErrorType.EmptyLastNameError452,
    ErrorType.LastNameTooLongError453,
    ErrorType.EmptyFirstNameError454,
    ErrorType.FirstNameTooLongError455,
    ErrorType.MiddleNameTooLongError456
  ],
  // Ошибки пароля
  PASSWORD: [
    ErrorType.EmptyPasswordError457,
    ErrorType.PasswordTooShortError458,
    ErrorType.InvalidPasswordFormatError459
  ],
  // Ошибки телефона
  PHONE: [
    ErrorType.EmptyPhoneNumberError460,
    ErrorType.InvalidPhoneFormatError461
  ],
};

// Группы кодов ошибок для водомеров
const METER_ERRORS = {
  // Ошибки заводского номера
  FACTORY_NUMBER: [
    ErrorType.EmptyFactoryNumberError473,
    ErrorType.FactoryNumberTooLongError474,
    ErrorType.InvalidFactoryNumberFormatError475
  ],
  // Ошибки даты установки
  FACTORY_YEAR: [
    ErrorType.EmptyFactoryYearError476,
    ErrorType.FutureFactoryYearError477
  ]
};

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
  const { showError, clearError } = useError();
  
  // Состояние для хранения ошибок валидации форм
  const [formErrors, setFormErrors] = useState<{
    apartmentNumber?: string;
    lastName?: string;
    firstName?: string;
    middleName?: string;
    password?: string;
    phoneNumber?: string;
  }>({});
  
  const [meterFormErrors, setMeterFormErrors] = useState<{
    factoryNumber?: string;
    factoryYear?: string;
  }>({});

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
    // Очищаем предыдущие ошибки формы
    setFormErrors({});
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
    // Очищаем предыдущие ошибки формы
    setMeterFormErrors({});
    setEditingMeter(meter.id);
    setEditMeterForm({
      factoryNumber: meter.factoryNumber,
      factoryYear: meter.factoryYear
    });
  };

  // Обработка ошибок от API, возвращает понятное сообщение для пользователя
  const getErrorMessageAndField = (error: any): { message: string, field?: string } => {
    console.log('Обработка ошибки:', error?.response?.data);
    console.log('Тип ошибки:', typeof error?.response?.data);
    if (error?.response?.data?.errors) {
      console.log('Ошибки валидации:', error.response.data.errors);
    }
    
    // Проверяем наличие кода ошибки в формате бэкенда
    const errorCode = error?.response?.data?.errorCode || error?.response?.data?.errorType;
    
    // Проверяем наличие ошибок валидации в стандартном формате ASP.NET Core
    const validationErrors = error?.response?.data?.errors;
    
    // Сначала проверяем, есть ли код ошибки (предпочтительный способ)
    if (errorCode) {
      // Определяем, к какому полю относится ошибка
      
      // Ошибки номера квартиры
      if (USER_ERRORS.APARTMENT_NUMBER.includes(errorCode)) {
        return { 
          message: getErrorMessage(errorCode), 
          field: 'apartmentNumber' 
        };
      }
      
      // Ошибки ФИО
      if (USER_ERRORS.NAME.includes(errorCode)) {
        const fieldMap: { [key: string]: string } = {
          [ErrorType.EmptyLastNameError452]: 'lastName',
          [ErrorType.LastNameTooLongError453]: 'lastName',
          [ErrorType.EmptyFirstNameError454]: 'firstName',
          [ErrorType.FirstNameTooLongError455]: 'firstName',
          [ErrorType.MiddleNameTooLongError456]: 'middleName'
        };
        return { 
          message: getErrorMessage(errorCode), 
          field: fieldMap[errorCode] 
        };
      }
      
      // Ошибки пароля
      if (USER_ERRORS.PASSWORD.includes(errorCode)) {
        return { 
          message: getErrorMessage(errorCode), 
          field: 'password' 
        };
      }
      
      // Ошибки телефона
      if (USER_ERRORS.PHONE.includes(errorCode)) {
        return { 
          message: getErrorMessage(errorCode), 
          field: 'phoneNumber' 
        };
      }
      
      // Ошибки водомеров - заводской номер
      if (METER_ERRORS.FACTORY_NUMBER.includes(errorCode)) {
        return { 
          message: getErrorMessage(errorCode), 
          field: 'factoryNumber' 
        };
      }
      
      // Ошибки водомеров - дата установки
      if (METER_ERRORS.FACTORY_YEAR.includes(errorCode)) {
        return { 
          message: getErrorMessage(errorCode), 
          field: 'factoryYear' 
        };
      }
      
      // Если код есть, но не определено поле, возвращаем только сообщение
      return { message: getErrorMessage(errorCode) };
    }
    
    // Обрабатываем стандартные ошибки валидации ASP.NET Core
    if (validationErrors && typeof validationErrors === 'object') {
      // Находим первое поле с ошибкой и его сообщение
      for (const [field, messages] of Object.entries(validationErrors)) {
        if (Array.isArray(messages) && messages.length > 0) {
          console.log(`Найдена ошибка валидации для поля ${field}:`, messages[0]);
          
          // Преобразуем имя поля в camelCase для соответствия нашим полям формы
          const fieldName = field.charAt(0).toLowerCase() + field.slice(1);
          
          // Пытаемся найти соответствующее пользовательское сообщение
          // Для FactoryNumber используем системные ошибки
          if (fieldName === 'factoryNumber') {
            // Предполагаем что это ошибка длины, так как это наиболее частая ошибка
            return {
              message: getErrorMessage(ErrorType.FactoryNumberTooLongError474),
              field: 'factoryNumber'
            };
          }
          
          return {
            message: messages[0],
            field: fieldName
          };
        }
      }
    }
    
    // Проверяем наличие общего сообщения об ошибке
    if (error?.response?.data?.title) {
      return { 
        message: error.response.data.title,
        field: undefined 
      };
    }
    
    // Если нет кода ошибки, возвращаем общее сообщение
    return { 
      message: error?.response?.data?.message || 
               error?.response?.data?.detail || 
               error?.message || 
               'Произошла ошибка при обновлении данных' 
    };
  };
  
  // Получение текста сообщения об ошибке по коду
  const getErrorMessage = (errorCode: string): string => {
    // Сообщения об ошибках соответствуют определениям из ErrorMessages.cs
    switch (errorCode) {
      // Ошибки номера квартиры
      case ErrorType.EmptyApartmentNumberError450: 
        return 'Номер квартиры не может быть пустым.';
      case ErrorType.InvalidApartmentNumberError451: 
        return 'Номер квартиры должен быть положительным числом.';
        
      // Ошибки ФИО
      case ErrorType.EmptyLastNameError452: 
        return 'Фамилия не может быть пустой.';
      case ErrorType.LastNameTooLongError453: 
        return 'Фамилия не может быть длиннее 50 символов.';
      case ErrorType.EmptyFirstNameError454: 
        return 'Имя не может быть пустым.';
      case ErrorType.FirstNameTooLongError455: 
        return 'Имя не может быть длиннее 50 символов.';
      case ErrorType.MiddleNameTooLongError456: 
        return 'Отчество не может быть длиннее 50 символов.';
        
      // Ошибки пароля
      case ErrorType.EmptyPasswordError457: 
        return 'Пароль не может быть пустым.';
      case ErrorType.PasswordTooShortError458: 
        return 'Пароль должен содержать минимум 8 символов.';
      case ErrorType.InvalidPasswordFormatError459: 
        return 'Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру.';
        
      // Ошибки телефона
      case ErrorType.EmptyPhoneNumberError460: 
        return 'Номер телефона не может быть пустым.';
      case ErrorType.InvalidPhoneFormatError461: 
        return 'Номер телефона должен быть в формате +7XXXXXXXXXX.';
        
      // Ошибки водомеров - заводской номер
      case ErrorType.EmptyFactoryNumberError473: 
        return 'Заводской номер счетчика не может быть пустым.';
      case ErrorType.FactoryNumberTooLongError474: 
        return 'Заводской номер счетчика не может быть длиннее 10 символов.';
      case ErrorType.InvalidFactoryNumberFormatError475: 
        return 'Заводской номер должен содержать только буквы и цифры.';
        
      // Ошибки водомеров - дата установки
      case ErrorType.EmptyFactoryYearError476: 
        return 'Дата установки счетчика не может быть пустой.';
      case ErrorType.FutureFactoryYearError477: 
        return 'Дата установки счетчика не может быть в будущем.';
        
      default:
        return 'Ошибка обработки запроса.';
    }
  };

  const handleSaveUser = async (userId: string) => {
    try {
      setUpdateLoading(true);
      setError(""); // Очищаем предыдущие ошибки
      setFormErrors({}); // Очищаем ошибки формы

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
      
      // Тестовая проверка: если пароль равен "test", имитируем ошибку валидации
      if (updatedUserData.password === "test") {
        console.log('Имитируем ошибку валидации пароля для тестирования');
        const testError = {
          response: {
            status: 400,
            data: {
              title: "One or more validation errors occurred.",
              status: 400,
              errors: {
                Password: [
                  "Пароль должен содержать как минимум одну заглавную букву, одну строчную букву и одну цифру."
                ]
              }
            }
          }
        };
        throw testError;
      }
      
      try {
        await updateUser(userId, updatedUserData);
      } catch (apiError: any) {
        // Обрабатываем ошибку API локально и не пропускаем её наверх
        if (apiError?.response?.status === 400) {
          console.log('Перехватили ошибку API 400:', apiError?.response?.data);
          
          // Обрабатываем ошибку, определяем поле и сообщение
          const { message, field } = getErrorMessageAndField(apiError);
          
          // Если определили поле с ошибкой, показываем ошибку рядом с полем
          if (field) {
            console.log('Поле с ошибкой:', field, 'Сообщение:', message);
            setFormErrors(prev => ({ ...prev, [field]: message }));
          } else {
            // Иначе показываем общую ошибку
            showError(message, 'warning');
          }
          
          // Прерываем выполнение функции, но не выбрасываем ошибку наверх
          setUpdateLoading(false);
          return;
        }
        
        // Если это не ошибка валидации, пробрасываем для обработки в блоке catch
        throw apiError;
      }
      
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
      
      showError("Данные пользователя успешно обновлены", 'success');
    } catch (err: any) {
      console.error('Ошибка при обновлении пользователя:', err);
      console.log('Детали ошибки:', err?.response?.data);
      
      // Обрабатываем ошибку, определяем поле и сообщение
      const { message, field } = getErrorMessageAndField(err);
      
      // Если определили поле с ошибкой, показываем ошибку рядом с полем
      if (field) {
        console.log('Поле с ошибкой:', field, 'Сообщение:', message);
        setFormErrors(prev => ({ ...prev, [field]: message }));
      } else {
        // Иначе показываем общую ошибку
        console.log('Общая ошибка:', message);
        showError(message, 'warning');
      }
      
    } finally {
      setUpdateLoading(false);
    }
  };

  const handleSaveMeter = async (meterId: string) => {
    try {
      setUpdateLoading(true);
      setError(""); // Очищаем предыдущие ошибки
      setMeterFormErrors({}); // Очищаем ошибки формы

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
      try {
        await updateWaterMeter(meterId, updatedMeterData);
      } catch (apiError: any) {
        // Обрабатываем ошибку API локально и не пропускаем её наверх
        if (apiError?.response?.status === 400) {
          console.log('Перехватили ошибку API 400 для счетчика:', apiError?.response?.data);
          console.log('Тип ошибки:', typeof apiError?.response?.data);
          console.log('Код ошибки:', apiError?.response?.data?.errorCode || apiError?.response?.data?.errorType);
          
          // Если это ошибка будущей даты, обрабатываем специальным образом
          if (apiError?.response?.data?.errorCode === ErrorType.FutureFactoryYearError477 || 
              apiError?.response?.data?.errorType === ErrorType.FutureFactoryYearError477) {
            console.log('Обнаружена ошибка будущей даты');
            setMeterFormErrors(prev => ({ 
              ...prev, 
              factoryYear: 'Дата установки счетчика не может быть в будущем.' 
            }));
            setUpdateLoading(false);
            return;
          }
          
          // Обрабатываем ошибку, определяем поле и сообщение
          const { message, field } = getErrorMessageAndField(apiError);
          
          // Если определили поле с ошибкой, показываем ошибку рядом с полем
          if (field) {
            console.log('Поле счетчика с ошибкой:', field, 'Сообщение:', message);
            setMeterFormErrors(prev => ({ ...prev, [field]: message }));
          } else {
            // Иначе показываем общую ошибку
            showError(message, 'warning');
          }
          
          // Прерываем выполнение функции, но не выбрасываем ошибку наверх
          setUpdateLoading(false);
          return;
        }
        
        // Если это не ошибка валидации, пробрасываем для обработки в блоке catch
        throw apiError;
      }
      
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
      showError('Данные счетчика успешно обновлены', 'success');
    } catch (err: any) {
      console.error('Ошибка при обновлении счетчика:', err);
      console.log('Детали ошибки счетчика:', err?.response?.data);
      
      // Обрабатываем ошибку, определяем поле и сообщение
      const { message, field } = getErrorMessageAndField(err);
      
      // Если определили поле с ошибкой, показываем ошибку рядом с полем
      if (field) {
        console.log('Поле счетчика с ошибкой:', field, 'Сообщение:', message);
        setMeterFormErrors(prev => ({ ...prev, [field]: message }));
      } else {
        // Иначе показываем общую ошибку
        console.log('Общая ошибка счетчика:', message);
        showError(message, 'warning');
      }
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
        showError("Ошибка загрузки данных пользователей.", 'error');
      } finally {
        setLoading(false);
      }
    };

    fetchUsersWithMeters();
    clearError(); // Очищаем ошибки при монтировании
  }, [clearError]);

  const getMeterTypeText = (type: number) => type === 1 ? "Горячая вода" : "Холодная вода";
  const getMeterLocationText = (location: number) => location === 1 ? "Кухня" : "Ванная";

  // Вспомогательная функция для отображения поля ввода с возможной ошибкой
  const renderInputField = (
    label: string, 
    name: keyof typeof formErrors, 
    value: any, 
    onChange: (value: any) => void, 
    type: string = 'text'
  ) => (
    <div>
      <label className="block text-sm font-medium text-gray-700">{label}</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        className={`mt-1 block w-full rounded-md border ${formErrors[name] ? 'border-red-500' : 'border-gray-300'} px-3 py-2 focus:outline-none ${formErrors[name] ? 'focus:border-red-500 focus:ring-red-500' : 'focus:border-blue-500 focus:ring-blue-500'}`}
      />
      {formErrors[name] && (
        <p className="mt-1 text-sm text-red-600">{formErrors[name]}</p>
      )}
    </div>
  );
  
  // Вспомогательная функция для отображения поля ввода счетчика с возможной ошибкой
  const renderMeterField = (
    label: string, 
    name: keyof typeof meterFormErrors, 
    value: any, 
    onChange: (value: any) => void, 
    type: string = 'text'
  ) => (
    <div>
      <label className="block text-sm font-medium text-gray-700">{label}</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        className={`mt-1 block w-full rounded-md border ${meterFormErrors[name] ? 'border-red-500' : 'border-gray-300'} px-3 py-2 focus:outline-none ${meterFormErrors[name] ? 'focus:border-red-500 focus:ring-red-500' : 'focus:border-blue-500 focus:ring-blue-500'}`}
      />
      {meterFormErrors[name] && (
        <p className="mt-1 text-sm text-red-600">{meterFormErrors[name]}</p>
      )}
    </div>
  );

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
                          {renderInputField(
                            'Номер квартиры', 
                            'apartmentNumber', 
                            editForm.apartmentNumber, 
                            (e) => setEditForm(prev => ({ ...prev, apartmentNumber: parseInt(e.target.value) })),
                            'number'
                          )}
                          
                          {renderInputField(
                            'Фамилия',
                            'lastName',
                            editForm.lastName,
                            (e) => setEditForm(prev => ({ ...prev, lastName: e.target.value }))
                          )}
                          
                          {renderInputField(
                            'Имя',
                            'firstName',
                            editForm.firstName,
                            (e) => setEditForm(prev => ({ ...prev, firstName: e.target.value }))
                          )}
                          
                          {renderInputField(
                            'Отчество',
                            'middleName',
                            editForm.middleName,
                            (e) => setEditForm(prev => ({ ...prev, middleName: e.target.value }))
                          )}
                          
                          {renderInputField(
                            'Телефон',
                            'phoneNumber',
                            editForm.phoneNumber,
                            (e) => setEditForm(prev => ({ ...prev, phoneNumber: e.target.value }))
                          )}
                          
                          {renderInputField(
                            'Пароль',
                            'password',
                            editForm.password,
                            (e) => setEditForm(prev => ({ ...prev, password: e.target.value })),
                            'password'
                          )}
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
                                {renderMeterField(
                                  'Заводской номер',
                                  'factoryNumber',
                                  editMeterForm.factoryNumber,
                                  (e) => setEditMeterForm(prev => ({ ...prev, factoryNumber: e.target.value })),
                                  'text'
                                )}
                                
                                {renderMeterField(
                                  'Дата установки',
                                  'factoryYear',
                                  new Date(editMeterForm.factoryYear as Date).toISOString().split('T')[0],
                                  (e) => {
                                    // Проверяем, не является ли дата будущей
                                    const selectedDate = new Date(e.target.value);
                                    const today = new Date();
                                    today.setHours(0, 0, 0, 0); // Сбрасываем время для корректного сравнения дат
                                    
                                    if (selectedDate > today) {
                                      // Если дата из будущего, устанавливаем ошибку сразу
                                      setMeterFormErrors(prev => ({ 
                                        ...prev, 
                                        factoryYear: 'Дата установки счетчика не может быть в будущем.' 
                                      }));
                                    } else {
                                      // Иначе очищаем ошибку для этого поля
                                      setMeterFormErrors(prev => {
                                        const newErrors = { ...prev };
                                        delete newErrors.factoryYear;
                                        return newErrors;
                                      });
                                    }
                                    
                                    setEditMeterForm(prev => ({ 
                                      ...prev, 
                                      factoryYear: new Date(e.target.value) 
                                    }));
                                  },
                                  'date'
                                )}
                                
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