import api from "./api";

export interface MeterReadingRequest {
    id: string;
    waterMeterId: string;
    waterValue: string;
    differenceValue: number;
    readingDate: string | Date;
}

export interface MeterReadingUpdateRequest {
    waterMeterId: string;
    waterValue: string;
    readingDate?: string | Date;
}

export const getAllMeterReading = async () => {
    const response = await api.get('/api/meter-readings');
    return response.data;
};

export const getMeterReadingByWaterMeterId = async (waterMeterId: string) => {
    const response = await api.get(`/api/meter-readings/by-water-meter/${waterMeterId}`);
    return response.data;
};

export const getMeterReadingById = async (id: string) => {
    const response = await api.get(`/api/meter-readings/${id}`);
    return response.data;
};

export const addMeterReading = async (meterReadingRequest: MeterReadingRequest) => {
    const response = await api.post('/api/meter-readings', meterReadingRequest);
    return response.data;
};

export const updateMeterReading = async (id: string, meterReadingRequest: MeterReadingUpdateRequest) => {
    const response = await api.put(`/api/meter-readings/${id}`, meterReadingRequest);
    return response.data;
};

export const deleteMeterReading = async (id: string) => {
    await api.delete(`/api/meter-readings/${id}`);
};