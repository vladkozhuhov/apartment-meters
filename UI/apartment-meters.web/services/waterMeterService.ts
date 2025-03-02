// waterMeterService.ts

import api from "./api";

export interface WaterMeterRequest {
    id: string;
    userId: string;
    placeOfWaterMeter: number;
    waterType: number;
    factoryNumber: number;
    factoryYear: Date;
}

export const getWaterMeterById = async (id: string) => {
    const response = await api.get(`/api/function/waterMeterById-get/${id}`);
    return response.data;
};

export const getWaterMetersByUserId = async (userId: string) => {
    const response = await api.get(`/api/function/waterMeterByUser-get/${userId}`);
    return response.data;
};

export const addWaterMeter = async (waterMeterRequest: WaterMeterRequest) => {
    await api.post('/api/function/waterMeter-add', waterMeterRequest);
};

export const updateWaterMeter = async (id: string, waterMeterRequest: WaterMeterRequest) => {
    await api.put(`/api/function/waterMeter-update/${id}`, waterMeterRequest);
};

export const deleteWaterMeter = async (id: string) => {
    await api.delete(`/api/function/waterMeter-delete/${id}`);
};