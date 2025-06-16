using System;

namespace DataAnalyzer.Csv.Models;

public class FinancialTransactionCsvModel
{
    /// <summary>
    /// Дата транзакции.
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Категория транзакции.
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Название транзакции.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Сумма транзакции.
    /// </summary>
    public decimal Amount { get; set; }
}