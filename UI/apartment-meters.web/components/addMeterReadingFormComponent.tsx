import React, { useState, useEffect } from 'react';
import { addMeterReading } from '../services/readingMeterService';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';

interface AddMeterReadingFormProps {
  userId: string;
  onSuccess: () => void;
  onCancel: () => void;
}

interface ReadingValue {
  whole: string;
  fraction: string;
}

const AddMeterReadingForm: React.FC<AddMeterReadingFormProps> = ({ userId, onSuccess, onCancel }) => {
  const [newReadings, setNewReadings] = useState<{ [key: string]: ReadingValue }>({});
  const [waterMeters, setWaterMeters] = useState<WaterMeterRequest[]>([]);

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
      }
    };

    fetchWaterMeters();
  }, [userId]);

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

  const handleAddReadings = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      for (const meter of waterMeters) {
        const { whole = "0", fraction = "0" } = newReadings[meter.id] || {};
        
        // Гарантируем, что у нас есть значения целой и дробной части
        const wholeValue = whole || "0";
        const fractionValue = fraction || "0";
        
        // Форматируем в формат, который ожидает API: целая часть, запятая, дробная часть
        const formattedValue = `${wholeValue},${fractionValue}`;
        
        console.log(`Отправка показаний для счетчика ${meter.id} со значением ${formattedValue}`);
        
        try {
          await addMeterReading({
            id: "",
            waterMeterId: meter.id,
            waterValue: formattedValue,
            differenceValue: 0,
            readingDate: new Date(),
          });
          console.log(`Успешно отправлены показания для счетчика ${meter.id}`);
        } catch (error: any) {
          console.error(`Ошибка при отправке показаний для счетчика ${meter.id}:`, error?.response?.data || error?.message || error);
          throw error;
        }
      }
      
      alert('Показания успешно добавлены!');
      onSuccess();
    } catch (error: any) {
      console.error('Ошибка при добавлении показаний:', error?.response?.data || error);
      alert(`Не удалось добавить показания: ${error?.response?.data?.title || error?.message || 'Проверьте формат данных.'}`);
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
    <form onSubmit={handleAddReadings} className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-50 flex justify-center items-center mt-6">
      <div className="bg-white p-6 rounded-lg shadow-lg w-[480px]">
        {Object.entries(groupedMeters).map(([place, meters]) => (
          <div key={place} className="mb-6">
            <h2 className="text-lg font-bold mb-4 pb-2 border-b border-gray-200">{place}</h2>
            {meters.map((meter) => (
              <div key={meter.id} className="mb-4 ml-2">
                <div className="flex items-center mb-2">
                  <div className="w-4 h-4 mr-3">
                    {meter.waterType === 1 ? (
                      <div className="w-full h-full rounded-full bg-red-500"></div>
                    ) : (
                      <div className="w-full h-full rounded-full bg-blue-500"></div>
                    )}
                  </div>
                  <h3 className="text-base font-medium text-gray-700">
                    {meter.waterType === 1 ? 'ГВС (компонент х/в)' : 'Холодное водоснабжение'}
                  </h3>
                </div>
                <div className="ml-7 flex items-center">
                  <input
                    id={`whole-${meter.id}`}
                    type="text"
                    value={newReadings[meter.id]?.whole || ''}
                    onChange={(e) => handleWholeInputChange(meter.id, e.target.value)}
                    onKeyDown={(e) => handleWholeInputKeyDown(meter.id, e)}
                    className="w-24 px-3 py-2 border border-gray-300 rounded-l text-base focus:border-blue-500 focus:outline-none text-right"
                    placeholder="00000"
                    required
                    maxLength={5}
                  />
                  <span className="px-2 py-2 bg-gray-100 border-t border-b border-gray-300 text-lg">,</span>
                  <input
                    id={`fraction-${meter.id}`}
                    type="text"
                    value={newReadings[meter.id]?.fraction || ''}
                    onChange={(e) => handleFractionInputChange(meter.id, e.target.value)}
                    className="w-20 px-3 py-2 border border-gray-300 rounded-r text-base focus:border-blue-500 focus:outline-none"
                    placeholder="000"
                    required
                    maxLength={3}
                  />
                  <span className="ml-2 text-gray-600">м³</span>
                </div>
              </div>
            ))}
          </div>
        ))}

        <div className="flex items-center justify-between mt-6">
          <button 
            type="submit" 
            className="bg-green-500 text-white px-5 py-2 rounded hover:bg-green-600 text-sm font-medium"
          >
            Сохранить
          </button>
          <button 
            type="button" 
            onClick={onCancel} 
            className="ml-4 bg-gray-500 text-white px-5 py-2 rounded hover:bg-gray-600 text-sm font-medium"
          >
            Отмена
          </button>
        </div>
      </div>
    </form>
  );
};

export default AddMeterReadingForm;