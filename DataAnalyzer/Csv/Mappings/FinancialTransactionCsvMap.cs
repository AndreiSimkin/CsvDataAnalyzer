using CsvHelper.Configuration;
using DataAnalyzer.Csv.Models;

namespace DataAnalyzer.Csv.Mappings;

public sealed class FinancialTransactionCsvMap : ClassMap<FinancialTransactionCsvModel>
{
    public FinancialTransactionCsvMap()
    {
        Map(m => m.Date).Index(0);
        Map(m => m.Category).Index(1);
        Map(m => m.Name).Index(2);
        Map(m => m.Amount).Index(3);
    }
}