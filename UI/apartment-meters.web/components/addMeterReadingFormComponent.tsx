import React, { useState } from 'react';
import { addMeterReading } from '../services/readingMeterService';

interface AddMeterReadingFormProps {
  userId: string;
  onSuccess: () => void;
  onCancel: () => void;
}

const AddMeterReadingForm: React.FC<AddMeterReadingFormProps> = ({ userId, onSuccess, onCancel }) => {
  const formatValue = (value: string) => value.padStart(5, '0');

  const [newReading, setNewReading] = useState({
    primaryColdWaterValue: "",
    primaryHotWaterValue: "",
    hasSecondaryMeter: false,
    secondaryColdWaterValue: "",
    secondaryHotWaterValue: "",
  });

  const handleInputChange = (field: string, value: string) => {
    const sanitizedValue = value.replace(/\D/g, ''); // Только цифры
    if (sanitizedValue.length <= 5) {
      setNewReading((prev) => ({ ...prev, [field]: sanitizedValue }));
    }
  };

  const handleCheckboxChange = () => {
    setNewReading((prev) => ({
      ...prev,
      hasSecondaryMeter: !prev.hasSecondaryMeter,
    }));
  };

  const handleAddReading = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      await addMeterReading(userId, {
        id: '',
        userId,
        primaryColdWaterValue: formatValue(newReading.primaryColdWaterValue || "0"),
        primaryHotWaterValue: formatValue(newReading.primaryHotWaterValue || "0"),
        primaryTotalValue: 0,
        primaryDifferenceValue: 0,
        hasSecondaryMeter: newReading.hasSecondaryMeter,
        secondaryColdWaterValue: newReading.hasSecondaryMeter ? formatValue(newReading.secondaryColdWaterValue || "0") : undefined,
        secondaryHotWaterValue: newReading.hasSecondaryMeter ? formatValue(newReading.secondaryHotWaterValue || "0") : undefined,
        readingDate: new Date(),
      });
      alert('Показания успешно добавлены!');
      onSuccess();
    } 
    catch (error) {
      console.error('Ошибка при добавлении показаний:', error);
      alert('Не удалось добавить показания');
    }
  };

  return (
    <form onSubmit={handleAddReading} className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-50 flex justify-center items-center mt-6 p-4 border rounded-lg">
      <div className="bg-white p-5 rounded shadow-lg w-2/5 max-w-4xl">

      <div className="mb-4">
        <label className="block font-semibold mb-2">Горячая вода (м³)</label>
        <input
          type="text"
          value={newReading.primaryHotWaterValue}
          onChange={(e) => handleInputChange('primaryHotWaterValue', e.target.value)}
          className="w-full px-3 py-2 border rounded"
          placeholder="Введите значение"
          required
        />
      </div>

      <div className="mb-4">
        <label className="block font-semibold mb-2">Холодная вода (м³)</label>
        <input
          type="text"
          value={newReading.primaryColdWaterValue}
          onChange={(e) => handleInputChange('primaryColdWaterValue', e.target.value)}
          className="w-full px-3 py-2 border rounded"
          placeholder="Введите значение"
          required
        />
      </div>

      <div className="mb-4">
        <label className="inline-flex items-center">
          <input type="checkbox" checked={newReading.hasSecondaryMeter} onChange={handleCheckboxChange} />
          <span className="ml-2">Добавить показания для второго счетчика</span>
        </label>
      </div>

      {newReading.hasSecondaryMeter && (
        <>
          <div className="mb-4">
            <label className="block font-semibold mb-2">Горячая вода (второй счетчик, м³)</label>
            <input
              type="text"
              value={newReading.secondaryHotWaterValue}
              onChange={(e) => handleInputChange('secondaryHotWaterValue', e.target.value)}
              className="w-full px-3 py-2 border rounded"
              placeholder="Введите значение"
              required
            />
          </div>

          <div className="mb-4">
            <label className="block font-semibold mb-2">Холодная вода (второй счетчик, м³)</label>
            <input
              type="text"
              value={newReading.secondaryColdWaterValue}
              onChange={(e) => handleInputChange('secondaryColdWaterValue', e.target.value)}
              className="w-full px-3 py-2 border rounded"
              placeholder="Введите значение"
              required
            />
          </div>
        </>
      )}

      <div className="flex items-center justify-between">
        <button type="submit" className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600">
          Сохранить
        </button>
        <button type="button" onClick={onCancel} className="ml-4 bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600">
          Отмена
        </button>
      </div>

      </div>
    </form>
  );
};

export default AddMeterReadingForm;