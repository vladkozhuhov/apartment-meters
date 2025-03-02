import React, { useState, useEffect } from 'react';
import { addMeterReading } from '../services/readingMeterService';
import { getWaterMetersByUserId, WaterMeterRequest } from '../services/waterMeterService';

interface AddMeterReadingFormProps {
  userId: string;
  onSuccess: () => void;
  onCancel: () => void;
}

const AddMeterReadingForm: React.FC<AddMeterReadingFormProps> = ({ userId, onSuccess, onCancel }) => {
  const formatValue = (value: string) => value.padStart(5, '0');

  const [newReadings, setNewReadings] = useState<{ [key: string]: string }>({});
  const [waterMeters, setWaterMeters] = useState<WaterMeterRequest[]>([]);

  useEffect(() => {
    const fetchWaterMeters = async () => {
      try {
        const meters = await getWaterMetersByUserId(userId);
        setWaterMeters(meters);
        const initialReadings: { [key: string]: string } = {};
        meters.forEach((meter: WaterMeterRequest) => {
          initialReadings[meter.id] = '';
        });
        setNewReadings(initialReadings);
      } catch (error) {
        console.error('Ошибка при получении счетчиков:', error);
      }
    };

    fetchWaterMeters();
  }, [userId]);

  const handleInputChange = (meterId: string, value: string) => {
    const sanitizedValue = value.replace(/\D/g, '');
    if (sanitizedValue.length <= 5) {
      setNewReadings((prev) => ({ ...prev, [meterId]: sanitizedValue }));
    }
  };

  const handleAddReadings = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await Promise.all(
        waterMeters.map(async (meter) => {
          await addMeterReading(meter.id, {
            id: '',
            waterMeterId: meter.id,
            waterValue: formatValue(newReadings[meter.id] || "0"),
            totalValue: 0,
            differenceValue: 0,
            readingDate: new Date(),
          });
        })
      );
      alert('Показания успешно добавлены!');
      onSuccess();
    } catch (error) {
      console.error('Ошибка при добавлении показаний:', error);
      alert('Не удалось добавить показания');
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
                <div className="ml-7">
                  <input
                    type="text"
                    value={newReadings[meter.id]}
                    onChange={(e) => handleInputChange(meter.id, e.target.value)}
                    className="w-full px-3 py-2 border border-gray-300 rounded text-base focus:border-blue-500 focus:outline-none"
                    placeholder="Новое показание"
                    required
                  />
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