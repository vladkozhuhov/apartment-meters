import api from "./api";

export interface MeterReadingRequest {
    id: string;
    userId: string;
    coldWaterValue: number;
    hotWaterValue: number;
    readingDate: Date;
}

export const getAllMeterReading = async () => {
    const response = await api.post('/api/WaterMeterReading');

    return response.data;
};

export const getMeterReadingByUserId = async (userId: string) => {
    const response = await api.post(`/api/WaterMeterReading/${userId}`);

    return response.data;
};

export const addMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post('/api/WaterMeterReading', meterReadingRequest);
};

export const updateMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post(`/api/WaterMeterReading/${id}`, meterReadingRequest);
};

export const deleteMeterReading = async (id: string) => {
    await api.post(`/api/WaterMeterReading/${id}`);
};