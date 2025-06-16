using System;
using SQLite;

namespace DataAnalyzer.Data.Models;

public class FinancialTransactionEntity
{
    /// <summary>
    /// Id транзакции.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement] 
    public int Id { get; set; }

    /// <summary>
    /// Дата транзакции.
    /// </summary>
    public DateTime Date { get; init; }

    /// <summary>
    /// Категория транзакции.
    /// </summary>
    public string? Category { get; init; }

    /// <summary>
    /// Название транзакции.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    /// Сумма транзакции.
    /// </summary>
    public decimal Amount { get; init; }
}