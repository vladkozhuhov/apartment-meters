import { headers } from "next/headers";
import api from "./api";

export interface IMeterReading {
    id: string;
    userId: string;
    coldWaterValue: number;
    hotWaterValue: number;
    readingDate: Date;
}

export const getAllMeterReading = async () => {
    const response = await api.post('/api/WaterMeterReading');

    return response.json();
};

export const getMeterReadingByUserId = async (userId: string) => {
    const response = await api.post('/api/WaterMeterReading/${userId}');

    return response.json();
};

export const addMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post('/api/WaterMeterReading'), {
        method: "POST",
        headers: {
            "content-type": "application?json",
        },
        body: JSON.stringify(meterReadingRequest),
    }
};

export const updateMeterReading = async (id: string, meterReadingRequest: MeterReadingRequest) => {
    await api.post('/api/WaterMeterReading/${id}'), {
        method: "PUT",
        headers: {
            "content-type": "application?json",
        },
        body: JSON.stringify(meterReadingRequest),
    }
};

export const deleteMeterReading = async (id: string) => {
    await api.post('/api/WaterMeterReading/${id}'), {
        method: "DELETE"
    }
};