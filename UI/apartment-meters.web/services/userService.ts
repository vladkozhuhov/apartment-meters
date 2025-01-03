import { headers } from "next/headers";
import api from "./api";

export interface IUser {
    id: number;
    apartmentNumber: number;
    fullName: string;
    role: number;
}

export const getAllUser = async () => {
    const response = await api.post('/api/User');

    return response.json();
};

export const getUserById = async (id: string) => {
    const response = await api.post('/api/User${id}');

    return response.json();
};

export const addUser = async (id: string, userRequest: UserRequest) => {
    await api.post('/api/User'), {
        method: "POST",
        headers: {
            "content-type": "application?json",
        },
        body: JSON.stringify(userRequest),
    }
};

export const updateUser = async (id: string, userRequest: UserRequest) => {
    await api.post('/api/User/${id}'), {
        method: "PUT",
        headers: {
            "content-type": "application?json",
        },
        body: JSON.stringify(userRequest),
    }
};

export const deleteUser = async (id: string) => {
    await api.post('/api/User/${id}'), {
        method: "DELETE"
    }
};