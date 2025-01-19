namespace Application.Services;

public class MeterReadingCalculatorService
{
    /// <summary>
    /// Вычисляет сумму показаний для счетчика.
    /// </summary>
    public int CalculateTotal(string coldWaterValue, string hotWaterValue)
    {
        return int.Parse(coldWaterValue) + int.Parse(hotWaterValue);
    }

    /// <summary>
    /// Вычисляет разницу между текущим и предыдущим значением.
    /// </summary>
    public int CalculateDifference(int currentTotal, int previousTotal)
    {
        return currentTotal - previousTotal;
    }
}