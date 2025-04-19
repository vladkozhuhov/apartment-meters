import React, { useState, useEffect } from 'react';
import { addMeterReading, getMeterReadingByWaterMeterId } from '../services/readingMeterService';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';
import { useError } from '../contexts/ErrorContext';
import { ErrorType } from '../hooks/useErrorHandler';

interface AddMeterReadingFormProps {
  userId: string;
  onSuccess: () => void;
  onCancel: () => void;
}

interface ReadingValue {
  whole: string;
  fraction: string;
}

// Перечисление кодов ошибок, которые мы обрабатываем
const METER_READING_ERRORS = {
  LESS_THAN_PREVIOUS: ErrorType.MeterReadingLessThanPreviousError353,
  INVALID_FORMAT: ErrorType.InvalidMeterReadingFormatError354,
  EMPTY_VALUE: ErrorType.EmptyWaterValueError480,
  INVALID_VALUE_FORMAT: ErrorType.InvalidWaterValueFormatError481,
  EMPTY_DATE: ErrorType.EmptyReadingDateError482,
  FUTURE_DATE: ErrorType.FutureReadingDateError483,
  OUTSIDE_ALLOWED_PERIOD: ErrorType.MeterReadingOutsideAllowedPeriodError484
};

const AddMeterReadingForm: React.FC<AddMeterReadingFormProps> = ({ userId, onSuccess, onCancel }) => {
  const [newReadings, setNewReadings] = useState<{ [key: string]: ReadingValue }>({});
  const [waterMeters, setWaterMeters] = useState<WaterMeterRequest[]>([]);
  const [loading, setLoading] = useState(false);
  const { showError, clearError } = useError();

  useEffect(() => {
    const fetchWaterMeters = async () => {
      try {
        const meters = await getWaterMetersByUserId(userId);
        setWaterMeters(meters);
        const initialReadings: { [key: string]: ReadingValue } = {};
        meters.forEach((meter: WaterMeterRequest) => {
          initialReadings[meter.id] = { whole: '', fraction: '' };
        });
        setNewReadings(initialReadings);
      } catch (error) {
        console.error('Ошибка при получении счетчиков:', error);
        showError('Не удалось загрузить данные счетчиков');
      }
    };

    fetchWaterMeters();
    // Очищаем ошибки при монтировании компонента
    clearError();
  }, [userId, clearError]);

  const handleWholeInputChange = (meterId: string, value: string) => {
    const sanitizedValue = value.replace(/\D/g, '');
    if (sanitizedValue.length <= 5) { // Максимум 5 цифр для целой части (согласно регулярке бэкенда)
      setNewReadings((prev) => ({ 
        ...prev, 
        [meterId]: { ...prev[meterId], whole: sanitizedValue } 
      }));
    }
  };

  const handleFractionInputChange = (meterId: string, value: string) => {
    const sanitizedValue = value.replace(/\D/g, '');
    if (sanitizedValue.length <= 3) { // Максимум 3 цифры для дробной части
      setNewReadings((prev) => ({ 
        ...prev, 
        [meterId]: { ...prev[meterId], fraction: sanitizedValue } 
      }));
    }
  };

  // Переход к следующему инпуту при заполнении первого
  const handleWholeInputKeyDown = (meterId: string, e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key !== 'Tab' && newReadings[meterId].whole.length === 5) {
      // Найти соответствующий инпут для дробной части и сфокусироваться на нем
      const fractionInput = document.getElementById(`fraction-${meterId}`);
      if (fractionInput) {
        e.preventDefault();
        fractionInput.focus();
      }
    }
  };

  // Обработка ошибок от API, возвращает понятное сообщение для пользователя
  const getErrorMessage = (error: any): string => {
    // Проверяем наличие кода ошибки в формате бэкенда
    const errorCode = error?.response?.data?.errorCode || error?.response?.data?.errorType;
    
    if (errorCode) {
      // Возвращаем конкретное сообщение для известных кодов ошибок
      switch (errorCode) {
        case METER_READING_ERRORS.LESS_THAN_PREVIOUS:
          return 'Новое показание не может быть меньше предыдущего.';
        case METER_READING_ERRORS.INVALID_FORMAT:
        case METER_READING_ERRORS.INVALID_VALUE_FORMAT:
          return 'Неверный формат показания счетчика. Формат должен быть "целое,дробное" (до 5 цифр до запятой и до 3 после).';
        case METER_READING_ERRORS.EMPTY_VALUE:
          return 'Показание счетчика не может быть пустым.';
        case METER_READING_ERRORS.EMPTY_DATE:
          return 'Дата показания не может быть пустой.';
        case METER_READING_ERRORS.FUTURE_DATE:
          return 'Дата показания не может быть в будущем.';
        case METER_READING_ERRORS.OUTSIDE_ALLOWED_PERIOD:
          return 'Показания можно подавать только с 23 по 25 число месяца.';
        default:
          // Используем сообщение от сервера, если оно есть
          return error?.response?.data?.message || 
                 error?.response?.data?.detail || 
                 'Ошибка при добавлении показаний.';
      }
    }
    
    // Если кода ошибки нет, но есть сообщение в ответе
    return error?.response?.data?.message || 
           error?.response?.data?.detail || 
           error?.message || 
           'Не удалось добавить показания. Пожалуйста, проверьте введенные данные.';
  };

  const handleAddReadings = async (e: React.FormEvent) => {
    e.preventDefault();
    clearError();
    setLoading(true);

    try {
      // Формируем массив показаний для всех счетчиков
      const readingsToSubmit = waterMeters.map(meter => {
        const { whole = "0", fraction = "0" } = newReadings[meter.id] || {};
        
        // Гарантируем, что у нас есть значения целой и дробной части
        const wholeValue = whole || "0";
        const fractionValue = fraction || "0";
        
        // Форматируем в формат, который ожидает API: целая часть, запятая, дробная часть
        const formattedValue = `${wholeValue},${fractionValue}`;
        
        return {
          id: "",
          waterMeterId: meter.id,
          waterValue: formattedValue,
          differenceValue: 0,
          readingDate: new Date(),
        };
      });
      
      // Проверяем и отправляем показания последовательно, но в случае ошибки прерываем весь процесс
      for (const reading of readingsToSubmit) {
        console.log(`Отправка показаний для счетчика ${reading.waterMeterId} со значением ${reading.waterValue}`);
        
        try {
          await addMeterReading(reading);
          console.log(`Успешно отправлены показания для счетчика ${reading.waterMeterId}`);
        } catch (error: any) {
          // Определяем код ошибки, если есть
          const errorCode = error?.response?.data?.errorCode || error?.response?.data?.errorType;
          
          console.error(`Ошибка при отправке показаний для счетчика ${reading.waterMeterId}:`, 
                       error?.response?.data || error?.message || error);
          
          // Для всех ошибок, связанных с показаниями, показываем сообщение и прерываем процесс
          const errorMessage = getErrorMessage(error);
          showError(errorMessage, 'warning');
          throw new Error(errorMessage); // Прерываем весь процесс
        }
      }
      
      // Если все показания успешно добавлены
      showError('Показания успешно добавлены!', 'success');
      onSuccess();
    } catch (error: any) {
      // Здесь обрабатываем ошибки, которые выбросили выше
      console.error('Ошибка при добавлении показаний:', error?.message || error);
      // Сообщение об ошибке уже показано выше, не показываем повторно
    } finally {
      setLoading(false);
    }
  };

  const groupedMeters = waterMeters.reduce((acc, meter) => {
    const place = meter.placeOfWaterMeter === 0 ? 'Ванная' : 'Кухня';
    if (!acc[place]) {
      acc[place] = [];
    }
    acc[place].push(meter);
    return acc;
  }, {} as { [key: string]: WaterMeterRequest[] });

  // Сортировка счетчиков по типу воды (сначала горячая, потом холодная)
  Object.keys(groupedMeters).forEach(place => {
    groupedMeters[place].sort((a, b) => b.waterType - a.waterType);
  });

  return (
    <form onSubmit={handleAddReadings} className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-50 flex justify-center items-center p-4 z-50">
      <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-[480px] max-h-[90vh] overflow-y-auto">
        {Object.entries(groupedMeters).map(([place, meters]) => (
          <div key={place} className="mb-4 sm:mb-6">
            <h2 className="text-base sm:text-lg font-bold mb-3 sm:mb-4 pb-2 border-b border-gray-200">{place}</h2>
            {meters.map((meter) => (
              <div key={meter.id} className="mb-3 sm:mb-4 ml-1 sm:ml-2">
                <div className="flex items-center mb-1 sm:mb-2">
                  <div className="w-3 sm:w-4 h-3 sm:h-4 mr-2 sm:mr-3">
                    {meter.waterType === 1 ? (
                      <div className="w-full h-full rounded-full bg-red-500"></div>
                    ) : (
                      <div className="w-full h-full rounded-full bg-blue-500"></div>
                    )}
                  </div>
                  <h3 className="text-sm sm:text-base font-medium text-gray-700">
                    {meter.waterType === 1 ? 'ГВС (компонент х/в)' : 'Холодное водоснабжение'}
                  </h3>
                </div>
                <div className="ml-5 sm:ml-7 flex items-center">
                  <input
                    id={`whole-${meter.id}`}
                    type="text"
                    inputMode="numeric"
                    value={newReadings[meter.id]?.whole || ''}
                    onChange={(e) => handleWholeInputChange(meter.id, e.target.value)}
                    onKeyDown={(e) => handleWholeInputKeyDown(meter.id, e)}
                    className="w-20 sm:w-24 px-2 sm:px-3 py-1 sm:py-2 border border-gray-300 rounded-l text-sm sm:text-base focus:border-blue-500 focus:outline-none text-right"
                    placeholder="00000"
                    required
                    maxLength={5}
                    disabled={loading}
                  />
                  <span className="px-1 sm:px-2 py-1 sm:py-2 bg-gray-100 border-t border-b border-gray-300 text-base sm:text-lg">,</span>
                  <input
                    id={`fraction-${meter.id}`}
                    type="text"
                    inputMode="numeric"
                    value={newReadings[meter.id]?.fraction || ''}
                    onChange={(e) => handleFractionInputChange(meter.id, e.target.value)}
                    className="w-16 sm:w-20 px-2 sm:px-3 py-1 sm:py-2 border border-gray-300 rounded-r text-sm sm:text-base focus:border-blue-500 focus:outline-none"
                    placeholder="000"
                    required
                    maxLength={3}
                    disabled={loading}
                  />
                  <span className="ml-1 sm:ml-2 text-sm sm:text-base text-gray-600">м³</span>
                </div>
              </div>
            ))}
          </div>
        ))}

        <div className="flex items-center justify-between mt-4 sm:mt-6">
          <button 
            type="button"
            onClick={onCancel}
            className="px-3 sm:px-4 py-1 sm:py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300 focus:outline-none text-sm sm:text-base"
            disabled={loading}
          >
            Отмена
          </button>
          <button 
            type="submit"
            className="px-3 sm:px-4 py-1 sm:py-2 bg-blue-500 text-white rounded hover:bg-blue-600 focus:outline-none flex items-center text-sm sm:text-base"
            disabled={loading}
          >
            {loading ? (
              <>
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Отправка...
              </>
            ) : (
              'Сохранить показания'
            )}
          </button>
        </div>
      </div>
    </form>
  );
};

export default AddMeterReadingForm;