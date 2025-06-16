using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using DataAnalyzer.Csv.Mappings;
using DataAnalyzer.Csv.Models;

namespace DataAnalyzer.Services;


/// <summary>
/// Сервис обработки .csv файлов
/// </summary>
public class CsvImportService : ICsvImportService
{
    public async IAsyncEnumerable<FinancialTransactionCsvModel> ReadFinancialTransactionsAsync(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            BufferSize = 8192,
            DetectDelimiter = true
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        csv.Context.RegisterClassMap<FinancialTransactionCsvMap>();

        await foreach (var record in csv.GetRecordsAsync<FinancialTransactionCsvModel>())
            yield return record;
    }
}