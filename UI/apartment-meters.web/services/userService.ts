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

export interface PaginatedUsersResponse {
    items: UserWithMetersAndReadings[];
    totalCount: number;
    page: number;
    pageSize: number;
    totalPages: number;
    hasNext: boolean;
    hasPrevious: boolean;
}

export interface UserWithMetersAndReadings {
    id: string;
    apartmentNumber: number;
    lastName: string;
    firstName: string;
    middleName: string;
    phoneNumber: string;
    role: number;
    waterMeters: WaterMeterWithReadings[];
}

export interface WaterMeterWithReadings {
    id: string;
    userId: string;
    placeOfWaterMeter: number;
    waterType: number;
    factoryNumber: string;
    factoryYear: Date;
    readings: MeterReading[];
}

export interface MeterReading {
    id: string;
    waterMeterId: string;
    waterValue: string;
    differenceValue: number;
    readingDate: Date;
}

export interface PhoneUpdateRequest {
    PhoneNumber: string;
}

export const getAllUser = async () => {
    const response = await api.get('/api/users');
    return response.data;
};

export const getPaginatedUsers = async (page: number = 1, pageSize: number = 30) => {
    try {
        console.log(`Получение пагинированных данных пользователей (страница: ${page}, размер: ${pageSize})`);
        const response = await api.get(`/api/users/paginated?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error) {
        console.error('Ошибка при получении пагинированных данных пользователей:', error);
        throw error;
    }
};

export const getPaginatedUsersWithMeters = async (page: number = 1, pageSize: number = 20) => {
    try {
        console.log(`Получение пагинированных данных пользователей с их счетчиками (страница: ${page}, размер: ${pageSize})`);
        const response = await api.get(`/api/users/admin/paginated?page=${page}&pageSize=${pageSize}`);
        return response.data;
    } catch (error: any) {
        console.error('Ошибка при получении пагинированных данных пользователей:', error);
        
        // Если ошибка 404 (данных нет), возвращаем пустой объект с правильной структурой
        if (error.response && error.response.status === 404) {
            console.log('Данных нет, возвращаем пустой объект');
            return {
                items: [],
                totalCount: 0,
                page: page,
                pageSize: pageSize,
                totalPages: 1,
                hasNext: false,
                hasPrevious: false
            };
        }
        
        throw error;
    }
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

export const updateUserPhone = async (userId: string, phoneNumber: string) => {
    try {
        console.log(`Обновление номера телефона пользователя ${userId}: ${phoneNumber}`);
        console.log('Отправляемый запрос:', { phoneNumber: phoneNumber });
        
        const response = await api.put(`/api/users/${userId}/phone`, { phoneNumber: phoneNumber });
        console.log('Успешный ответ при обновлении телефона:', response);
        return response.data;
    } catch (error: any) {
        console.error('Ошибка при обновлении номера телефона:', error);
        console.error('Детали ошибки:', error?.response);
        if (error.response) {
            console.error('Статус ошибки:', error.response.status);
            console.error('Данные ответа:', error.response.data);
            console.error('Заголовки ответа:', error.response.headers);
        }
        if (error.request) {
            console.error('Данные запроса:', error.request);
        }
        throw error;
    }
};