import React, { useEffect, useState } from "react";
import { getAllUser, UserRequest } from "../services/userService";

interface UsersListProps {
  onClose: () => void;
}

const UsersList: React.FC<UsersListProps> = ({ onClose }) => {
  const [users, setUsers] = useState<UserRequest[]>([]);
  const [filteredUser, setFilteredUser] = useState<UserRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [filter, setFilter] = useState({ apartment: ''});  

  const handleFilterChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFilter((prev) => ({ ...prev, [name]: value }));
  };

  const applyFilter = () => {
    let result = users;
    if (filter.apartment) {
      result = result.filter((r) => r.id === filter.apartment);
    }
    setFilteredUser(result);
  };

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await getAllUser();
        setUsers(data);
      } catch (err) {
        setError("Ошибка загрузки пользователей.");
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  return (
    <div className="fixed top-0 left-0 w-full h-full bg-black bg-opacity-50 flex justify-center items-center">
      <div className="bg-white p-5 rounded shadow-lg w-3/4 max-w-4xl">
        <label>
          Номер квартиры:
          <input
            type="text"
            name="apartment"
            value={filter.apartment}
            onChange={handleFilterChange}
            className="border px-2 py-1 ml-2"
          />
        </label>
        <button onClick={applyFilter} className="ml-4 bg-blue-500 text-white px-4 py-2 rounded">
          Применить фильтр
        </button>
        <div className="flex justify-between items-center mb-4">
          <h2 className="text-xl font-bold">Список пользователей</h2>
          <button onClick={onClose} className="text-red-500 text-xl">✖</button>
        </div>

        {loading ? (
          <p>Загрузка...</p>
        ) : error ? (
          <p className="text-red-500">{error}</p>
        ) : (
          <table className="w-full border-collapse border border-gray-300">
            <thead>
              <tr className="bg-gray-100">
                <th className="border px-4 py-2">№ Квартиры</th>
                <th className="border px-4 py-2">ФИО</th>
                <th className="border px-4 py-2">Пароль</th>
                <th className="border px-4 py-2">Телефон</th>
                <th className="border px-4 py-2">Роль</th>
                <th className="border px-4 py-2">Заводской номер сч.</th>
                <th className="border px-4 py-2">Дата выпуска сч.</th>
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td className="border px-4 py-2">{user.apartmentNumber}</td>
                  <td className="border px-4 py-2">
                    {user.lastName} {user.firstName} {user.middleName}
                  </td>
                  <td className="border px-4 py-2">{user.password}</td>
                  <td className="border px-4 py-2">{user.phoneNumber}</td>
                  <td className="border px-4 py-2">{user.role === 1 ? "Админ" : "Пользователь"}</td>
                  <td className="border px-4 py-2">{user.factoryNumber}</td>
                  <td className="border px-4 py-2">
                    {new Date(user.factoryYear).toLocaleDateString()}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
};

export default UsersList;