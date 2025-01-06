import api from "./api";

export interface MeterReadingRequest {
    id: string;
    userId: string;
    coldWaterValue: number;
    hotWaterValue: number;
    readingDate: Date;
}

export const getAllMeterReading = async () => {
    const response = await api.get('/api/WaterMeterReading');

    return response.data;
};

export const getMeterReadingByUserId = async (userId: string) => {
    const response = await api.get(`/api/WaterMeterReading/by-user/${userId}`);

    return response.data;
};

export const getMeterReadingById = async (id: string) => {
    const response = await api.get(`/api/WaterMeterReading/by-id/${id}`);

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