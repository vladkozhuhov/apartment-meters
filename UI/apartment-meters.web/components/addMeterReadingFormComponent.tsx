import React, { useState } from 'react';
import { addMeterReading } from '../services/readingMeterService';

interface AddMeterReadingFormProps {
  userId: string;
  onSuccess: () => void; // Функция для обновления списка показаний после добавления
  onCancel: () => void;  // Функция для закрытия формы
}

const AddMeterReadingForm: React.FC<AddMeterReadingFormProps> = ({ userId, onSuccess, onCancel }) => {
  const [newReading, setNewReading] = useState({
    coldWaterValue: 0,
    hotWaterValue: 0
  });

  const handleAddReading = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await addMeterReading(userId, {
        id: '', // ID генерируется на сервере
        userId,
        coldWaterValue: newReading.coldWaterValue,
        hotWaterValue: newReading.hotWaterValue,
        readingDate: new Date()
      });
      alert('Показания успешно добавлены!');
      onSuccess(); // Вызываем onSuccess для обновления данных
    } catch (error) {
      console.error('Ошибка при добавлении показаний:', error);
      alert('Не удалось добавить показания');
    }
  };

  return (
    <form onSubmit={handleAddReading} className="mt-6 p-4 border rounded-lg">
      <div className="mb-4">
        <label className="block font-semibold mb-2" htmlFor="hotWaterValue">
          Горячая вода (м³)
        </label>
        <input
          type="number"
          id="hotWaterValue"
          value={newReading.hotWaterValue}
          onChange={(e) => setNewReading({ ...newReading, hotWaterValue: Number(e.target.value) })}
          className="w-full px-3 py-2 border rounded"
          required
        />
      </div>

      <div className="mb-4">
        <label className="block font-semibold mb-2" htmlFor="coldWaterValue">
          Холодная вода (м³)
        </label>
        <input
          type="number"
          id="coldWaterValue"
          value={newReading.coldWaterValue}
          onChange={(e) => setNewReading({ ...newReading, coldWaterValue: Number(e.target.value) })}
          className="w-full px-3 py-2 border rounded"
          required
        />
      </div>

      <button type="submit" className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600">
        Сохранить
      </button>
      <button
        type="button"
        onClick={onCancel}
        className="ml-4 bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
      >
        Отмена
      </button>
    </form>
  );
};

export default AddMeterReadingForm;