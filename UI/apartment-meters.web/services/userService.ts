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
    factoryNumber: string;
    factoryYear: Date;
}

export const getAllUser = async () => {
    const response = await api.get('/api/function/users-get');

    return response.data;
};

export const getUserById = async (id: string) => {
    const response = await api.get(`/api/function/user-get/${id}`);

    return response.data;
};

export const addUser = async (id: string, userRequest: UserRequest) => {
    await api.post('/api/function/user-add', userRequest);
};

export const updateUser = async (id: string, userRequest: UserRequest) => {
    await api.put(`/api/function/user-update/${id}`, userRequest);
};

export const deleteUser = async (id: string) => {
    await api.delete(`/api/function/user-delete/${id}`);
};