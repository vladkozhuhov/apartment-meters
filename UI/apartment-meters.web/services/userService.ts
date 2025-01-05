import api from "./api";

export interface UserRequest {
    id: number;
    apartmentNumber: number;
    fullName: string;
    role: number;
}

export const getAllUser = async () => {
    const response = await api.get('/api/User');

    return response.data;
};

export const getUserById = async (id: string) => {
    const response = await api.get(`/api/User/${id}`);

    return response.data;
};

export const addUser = async (id: string, userRequest: UserRequest) => {
    await api.post('/api/User', userRequest);
};

export const updateUser = async (id: string, userRequest: UserRequest) => {
    await api.put(`/api/User/${id}`, userRequest);
};

export const deleteUser = async (id: string) => {
    await api.delete(`/api/User/${id}`);
};