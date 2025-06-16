using System.Collections.Generic;
using DataAnalyzer.Csv.Models;

namespace DataAnalyzer.Services;

/// <summary>
/// Сервис обработки .csv файлов
/// </summary>
public interface ICsvImportService
{
    IAsyncEnumerable<FinancialTransactionCsvModel> ReadFinancialTransactionsAsync(string filePath);
}