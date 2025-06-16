using System.Globalization;

namespace DataAnalyzer.Helpers;

public static class SizeConverter
{
    // Русские сокращения для байтов, килобайтов, мегабайтов и т. д.
    private static readonly string[] SizeSuffixes = { "Б", "КБ", "МБ", "ГБ", "ТБ", "ПБ", "ЭБ", "ЗБ", "ЙБ" };

    /// <summary>
    /// Конвертирует число в удобочитаемый размер с русскими единицами.
    /// </summary>
    /// <param name="bytes">Размер в байтах.</param>
    public static string Convert(float bytes)
    {
        if (float.IsNaN(bytes) || float.IsInfinity(bytes))
            return bytes.ToString(CultureInfo.InvariantCulture);

        int baseUnit = 1024;
        int magnitude = 0;
        float adjustedSize = bytes;

        while (adjustedSize >= baseUnit && magnitude < SizeSuffixes.Length - 1)
        {
            adjustedSize /= baseUnit;
            magnitude++;
        }

        // Форматируем без .0, если число целое, иначе с одним знаком после запятой
        string format = (adjustedSize % 1 == 0) ? "0" : "0.00";
        return adjustedSize.ToString(format) + " " + SizeSuffixes[magnitude];
    }
}