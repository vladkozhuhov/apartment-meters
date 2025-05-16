import React, { useState, useEffect } from 'react';
import { updateMeterReading, getMeterReadingById } from '../services/readingMeterService';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';
import { useError } from '../contexts/ErrorContext';
import { ErrorType } from '../hooks/useErrorHandler';

interface EditMeterReadingProps {
  readingId: string;
  onSuccess: () => void;
  onCancel: () => void;
}

interface ReadingValue {
  whole: string;
  fraction: string;
}

interface MeterReadingFullInfo {
  id: string;
  waterMeterId: string;
  waterValue: string;
  differenceValue: number;
  readingDate: Date;
  waterMeter?: {
    factoryNumber: string;
    waterType: number;
    placeOfWaterMeter: number;
  };
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

const EditMeterReadingComponent: React.FC<EditMeterReadingProps> = ({ readingId, onSuccess, onCancel }) => {
  const [readingValue, setReadingValue] = useState<ReadingValue>({ whole: '', fraction: '' });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [meterReadingInfo, setMeterReadingInfo] = useState<MeterReadingFullInfo | null>(null);
  const { showError, clearError } = useError();

  // Загружаем данные показания при открытии компонента
  useEffect(() => {
    const fetchReadingData = async () => {
      try {
        setLoading(true);
        const readingData = await getMeterReadingById(readingId);
        setMeterReadingInfo(readingData);

        // Разделяем значение показания на целую и дробную часть
        const valueParts = readingData.waterValue.split(',');
        setReadingValue({
          whole: valueParts[0] || '',
          fraction: valueParts[1] || ''
        });
      } catch (error) {
        console.error('Ошибка при загрузке данных показания:', error);
        showError('Не удалось загрузить данные показания', 'error');
      } finally {
        setLoading(false);
      }
    };

    fetchReadingData();
    clearError(); // Очищаем ошибки при монтировании компонента
  }, [readingId, clearError]);

  const handleWholeInputChange = (value: string) => {
    // Разрешаем только цифры, сохраняя ведущие нули
    const sanitizedValue = value.replace(/[^\d]/g, '');
    if (sanitizedValue.length <= 5) { // Максимум 5 цифр для целой части
      setReadingValue((prev) => ({ ...prev, whole: sanitizedValue }));
    }
  };

  const handleFractionInputChange = (value: string) => {
    const sanitizedValue = value.replace(/\D/g, '');
    if (sanitizedValue.length <= 3) { // Максимум 3 цифры для дробной части
      setReadingValue((prev) => ({ ...prev, fraction: sanitizedValue }));
    }
  };

  // Переход к следующему инпуту при заполнении первого
  const handleWholeInputKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    // Перемещаем фокус только если нажата цифровая клавиша (не backspace, delete и т.д.)
    // и если после ввода длина будет равна 5
    const isDigit = /^\d$/.test(e.key);
    const inputElement = e.target as HTMLInputElement;
    const willReachMaxLength = isDigit && inputElement.selectionStart === inputElement.value.length && inputElement.value.length === 4;
    
    if (e.key !== 'Tab' && willReachMaxLength) {
      // Найти соответствующий инпут для дробной части и сфокусироваться на нем
      const fractionInput = document.getElementById(`edit-fraction-input`);
      if (fractionInput) {
        // Дожидаемся ввода символа перед переходом
        setTimeout(() => {
          fractionInput.focus();
        }, 10);
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
                 'Ошибка при обновлении показаний.';
      }
    }
    
    // Если кода ошибки нет, но есть сообщение в ответе
    return error?.response?.data?.message || 
           error?.response?.data?.detail || 
           error?.message || 
           'Не удалось обновить показания. Пожалуйста, проверьте введенные данные.';
  };

  const getMeterTypeText = (type: number) => type === 1 ? "Горячая вода" : "Холодная вода";
  const getMeterLocationText = (location: number) => location === 1 ? "Кухня" : "Ванная";

  const handleSaveReading = async (e: React.FormEvent) => {
    e.preventDefault();
    clearError();
    setSaving(true);

    try {
      if (!meterReadingInfo) return;

      // Форматируем в формат, который ожидает API: целая часть, запятая, дробная часть
      const formattedValue = `${readingValue.whole || '0'},${readingValue.fraction || '0'}`;
      
      // Проверяем, не стало ли новое значение меньше текущего
      const currentValue = parseFloat(meterReadingInfo.waterValue.replace(',', '.'));
      const newValue = parseFloat(formattedValue.replace(',', '.'));
      
      if (newValue < currentValue) {
        showError('Новое показание не может быть меньше предыдущего', 'warning');
        setSaving(false);
        return;
      }
      
      // Подготавливаем данные для обновления - только значение показания
      const readingToSend = {
        waterMeterId: meterReadingInfo.waterMeterId,
        waterValue: formattedValue
      };
      
      console.log(`Отправка обновленного показания: ${JSON.stringify(readingToSend)}`);
      await updateMeterReading(readingId, readingToSend);
      
      showError('Показания успешно обновлены!', 'success');
      onSuccess();
    } catch (error: any) {
      const errorMessage = getErrorMessage(error);
      console.error('Ошибка при обновлении показаний:', error?.message || error);
      showError(errorMessage, 'warning');
    } finally {
      setSaving(false);
    }
  };

  // Форматируем дату показания для отображения
  const formattedDate = meterReadingInfo?.readingDate 
    ? new Date(meterReadingInfo.readingDate).toLocaleDateString('ru-RU', {
        year: 'numeric',
        month: 'long',
        day: 'numeric'
      })
    : '';
  
  // Получаем информацию о водомере для отображения
  const meterInfo = meterReadingInfo?.waterMeter
    ? `${getMeterTypeText(meterReadingInfo.waterMeter.waterType)} - ${getMeterLocationText(meterReadingInfo.waterMeter.placeOfWaterMeter)}`
    : 'Загрузка...';

  return (
    <form onSubmit={handleSaveReading} className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-80 backdrop-blur-sm flex justify-center items-center p-4 z-50">
      <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-[480px] max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center mb-4 pb-2 border-b border-gray-200">
          <h2 className="text-lg sm:text-xl font-bold">Редактирование показания</h2>
          <button
            type="button"
            onClick={onCancel}
            className="text-gray-500 hover:text-gray-700 focus:outline-none"
            title="Закрыть"
          >
            <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
        
        {loading ? (
          <div className="flex justify-center items-center py-8">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <>
            {/* Информация о водомере и показании */}
            <div className="mb-4 p-3 bg-gray-50 rounded-md">
              <div className="flex items-center mb-2">
                <div className="w-3 sm:w-4 h-3 sm:h-4 mr-2 sm:mr-3">
                  {meterReadingInfo?.waterMeter?.waterType === 1 ? (
                    <div className="w-full h-full rounded-full bg-red-500"></div>
                  ) : (
                    <div className="w-full h-full rounded-full bg-blue-500"></div>
                  )}
                </div>
                <h3 className="font-medium">{meterInfo}</h3>
              </div>
              <p className="text-sm text-gray-600 mb-1">
                Текущее значение: {meterReadingInfo?.waterValue} м³
              </p>
              <p className="text-sm text-gray-600">
                Дата показания: {formattedDate}
              </p>
            </div>
            
            {/* Поле для ввода нового значения */}
            <div className="mb-4">
              <label className="block text-sm font-medium text-gray-700 mb-2">Новое значение показания:</label>
              <div className="flex items-center">
                <input
                  id="edit-whole-input"
                  type="text"
                  inputMode="numeric"
                  value={readingValue.whole}
                  onChange={(e) => handleWholeInputChange(e.target.value)}
                  onKeyDown={(e) => handleWholeInputKeyDown(e)}
                  className="w-20 sm:w-24 px-2 sm:px-3 py-1 sm:py-2 border border-gray-300 rounded-l text-sm sm:text-base focus:border-blue-500 focus:outline-none text-right"
                  placeholder="00000"
                  required
                  maxLength={5}
                  disabled={saving}
                />
                <span className="px-1 sm:px-2 py-1 sm:py-2 bg-gray-100 border-t border-b border-gray-300 text-base sm:text-lg">,</span>
                <input
                  id="edit-fraction-input"
                  type="text"
                  inputMode="numeric"
                  value={readingValue.fraction}
                  onChange={(e) => handleFractionInputChange(e.target.value)}
                  className="w-16 sm:w-20 px-2 sm:px-3 py-1 sm:py-2 border border-gray-300 rounded-r text-sm sm:text-base focus:border-blue-500 focus:outline-none"
                  placeholder="000"
                  required
                  maxLength={3}
                  disabled={saving}
                />
                <span className="ml-1 sm:ml-2 text-sm sm:text-base text-gray-600">м³</span>
              </div>
            </div>
          </>
        )}

        <div className="flex items-center justify-between mt-4 sm:mt-6">
          <button 
            type="button"
            onClick={onCancel}
            className="px-3 sm:px-4 py-1 sm:py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300 focus:outline-none text-sm sm:text-base"
            disabled={saving || loading}
          >
            Отмена
          </button>
          <button 
            type="submit"
            className="px-3 sm:px-4 py-1 sm:py-2 bg-blue-500 text-white rounded hover:bg-blue-600 focus:outline-none flex items-center text-sm sm:text-base"
            disabled={saving || loading}
          >
            {saving ? (
              <>
                <svg className="animate-spin -ml-1 mr-2 h-4 w-4 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                Сохранение...
              </>
            ) : (
              'Сохранить изменения'
            )}
          </button>
        </div>
      </div>
    </form>
  );
};

export default EditMeterReadingComponent; 