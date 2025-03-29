// waterMeterService.ts

import api from "./api";

export interface WaterMeterRequest {
    id: string;
    userId: string;
    placeOfWaterMeter: number;
    waterType: number;
    factoryNumber: string;
    factoryYear: Date;
}

export interface WaterMeterUpdateRequest {
    id: string;
    userId?: string;
    placeOfWaterMeter?: number;
    waterType?: number;
    factoryNumber?: string;
    factoryYear?: Date;
}

export const getWaterMeterById = async (id: string) => {
    const response = await api.get(`/api/water-meters/${id}`);
    return response.data;
};

export const getWaterMetersByUserId = async (userId: string) => {
    const response = await api.get(`/api/water-meters/by-user/${userId}`);
    return response.data;
};

export const addWaterMeter = async (waterMeterRequest: WaterMeterRequest) => {
    const response = await api.post('/api/water-meters', waterMeterRequest);
    return response.data;
};

export const updateWaterMeter = async (id: string, waterMeterRequest: WaterMeterUpdateRequest) => {
    // Создаем DTO объект для отправки на бэкенд
    const waterMeterUpdateDto = {
        Id: waterMeterRequest.id,
        ...(waterMeterRequest.userId && { UserId: waterMeterRequest.userId }),
        ...(waterMeterRequest.placeOfWaterMeter !== undefined && { 
            PlaceOfWaterMeter: waterMeterRequest.placeOfWaterMeter 
        }),
        ...(waterMeterRequest.waterType !== undefined && { 
            WaterType: waterMeterRequest.waterType 
        }),
        ...(waterMeterRequest.factoryNumber !== undefined && { 
            // Заводской номер должен быть строкой
            FactoryNumber: waterMeterRequest.factoryNumber
        }),
        ...(waterMeterRequest.factoryYear !== undefined && { 
            // Преобразуем дату в формат ISO с отсечением времени (только дата)
            FactoryYear: waterMeterRequest.factoryYear instanceof Date 
                ? waterMeterRequest.factoryYear.toISOString().split('T')[0] 
                : waterMeterRequest.factoryYear 
        })
    };
    
    console.log('Отправка данных на сервер:', waterMeterUpdateDto);
    
    // Отправляем только сам DTO объект, без обертки
    const response = await api.put(`/api/water-meters/${id}`, waterMeterUpdateDto);
    return response.data;
};

export const deleteWaterMeter = async (id: string) => {
    await api.delete(`/api/water-meters/${id}`);
};