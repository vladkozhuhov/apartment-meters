import React, { useEffect, useState } from "react";
import styles from "./UserDashboard.module.css";
import api from "../../services/api";

// Тип для данных о показаниях водомеров
interface WaterMeterReading {
  id: string;
  date: string;
  coldWater: number;
  hotWater: number;
}

const UserDashboard: React.FC = () => {
  const [readings, setReadings] = useState<WaterMeterReading[]>([]);
  const [newReading, setNewReading] = useState({
    coldWater: "",
    hotWater: "",
  });

  useEffect(() => {
    // Функция для получения данных о показаниях водомеров
    const fetchReadings = async () => {
      try {
        const response = await api.get<WaterMeterReading[]>("/api/WaterMeterReading");
        setReadings(response.data);
      } catch (error) {
        console.error("Ошибка при получении показаний водомеров:", error);
      }
    };

    fetchReadings();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setNewReading({ ...newReading, [name]: value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    try {
      // Отправка нового показания
      await api.post("/api/WaterMeterReading", {
        coldWater: parseFloat(newReading.coldWater),
        hotWater: parseFloat(newReading.hotWater),
      });

      // Обновляем список после успешного добавления
      const response = await api.get<WaterMeterReading[]>("/api/WaterMeterReading");
      setReadings(response.data);

      // Сбрасываем форму
      setNewReading({ coldWater: "", hotWater: "" });
    } catch (error) {
      console.error("Ошибка при отправке нового показания:", error);
    }
  };

  return (
    <div className={styles.dashboard}>
      <h1>Личный кабинет</h1>
      <div className={styles.history}>
        <h2>История показаний</h2>
        <table className={styles.table}>
          <thead>
            <tr>
              <th>Дата</th>
              <th>Холодная вода</th>
              <th>Горячая вода</th>
            </tr>
          </thead>
          <tbody>
            {readings.map((reading) => (
              <tr key={reading.id}>
                <td>{new Date(reading.date).toLocaleDateString()}</td>
                <td>{reading.coldWater}</td>
                <td>{reading.hotWater}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <div className={styles.form}>
        <h2>Добавить новое показание</h2>
        <form onSubmit={handleSubmit}>
          <div className={styles.inputGroup}>
            <label htmlFor="coldWater">Холодная вода</label>
            <input
              type="number"
              id="coldWater"
              name="coldWater"
              value={newReading.coldWater}
              onChange={handleInputChange}
              required
            />
          </div>
          <div className={styles.inputGroup}>
            <label htmlFor="hotWater">Горячая вода</label>
            <input
              type="number"
              id="hotWater"
              name="hotWater"
              value={newReading.hotWater}
              onChange={handleInputChange}
              required
            />
          </div>
          <button type="submit">Отправить</button>
        </form>
      </div>
    </div>
  );
};

export default UserDashboard;