import api from "./api";

export interface UserRequest {
    id: string;
    apartmentNumber: number;
    lastName: string;
    firstName: string;
    middleName: string;
    password: string;
    phoneNumber: string;
    role: number;
}

export const getAllUser = async () => {
    const response = await api.get('/api/users');
    return response.data;
};

export const getUserById = async (id: string) => {
    const response = await api.get(`/api/users/${id}`);
    return response.data;
};

export const getUserByApartmentNumber = async (apartmentNumber: string) => {
    try {
        console.log(`Запрос пользователя по номеру квартиры: ${apartmentNumber}`);
        const response = await api.get(`/api/users/by-apartment/${apartmentNumber}`);
        return response.data;
    } catch (error) {
        console.error('Ошибка при получении пользователя по номеру квартиры:', error);
        throw error;
    }
};

export const addUser = async (userRequest: UserRequest) => {
    const response = await api.post('/api/users', userRequest);
    return response.data;
};

export const updateUser = async (id: string, userRequest: UserRequest) => {
    console.log(`Отправка запроса на обновление пользователя ${id}:`, userRequest);
    try {
        const response = await api.put(`/api/users/${id}`, userRequest);
        console.log('Успешный ответ:', response.data);
        return response.data;
    } catch (error: any) {
        console.error('Ошибка при обновлении пользователя:', error);
        console.log('Детали ошибки:', error?.response?.data);
        throw error;
    }
};

export const deleteUser = async (id: string) => {
    await api.delete(`/api/users/${id}`);
};