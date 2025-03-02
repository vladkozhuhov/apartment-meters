import api from "./api";

export interface MeterReadingRequest {
    id: string;
    waterMeterId: string;
    waterValue: string;
    totalValue: number;
    differenceValue: number;
    readingDate: Date;
}

export const getAllMeterReading = async () => {
    const response = await api.get('/api/function/meterReadings-get');

    return response.data;
};

export const getMeterReadingByWaterMeterId = async (waterMeterId: string) => {
    const response = await api.get(`/api/function/meterReadingByWaterMeterId-get/${waterMeterId}`);

    return response.data;
};

export const getMeterReadingById = async (id: string) => {
    const response = await api.get(`/api/function/meterReadingById-get/${id}`);

    return response.data;
};

export const addMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post('/api/function/meterReading-add', meterReadingRequest);
};

export const updateMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post(`/api/function/meterReading-update/${id}`, meterReadingRequest);
};

export const deleteMeterReading = async (id: string) => {
    await api.post(`/api/function/meterReading-delete/${id}`);
};