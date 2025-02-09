interface IMeterReading {
    id: string;
    userId: string;
    primaryColdWaterValue: string;
    primaryHotWaterValue: string;
    primaryTotalValue: number;
    primaryDifferenceValue: number;
    hasSecondaryMeter: boolean;
    secondaryColdWaterValue?: string;
    secondaryHotWaterValue?: string;
    secondaryTotalValue?: number;
    secondaryDifferenceValue?: number;
    readingDate: Date;
}