using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DataAnalyzer.Csv.Models;
using DataAnalyzer.Data.Models;
using DataAnalyzer.Data.Repositories;
using DataAnalyzer.Helpers;
using DataAnalyzer.Models;
using DataAnalyzer.Services;

namespace DataAnalyzer.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly ICsvImportService _csvImportService;
    private readonly IFileDialogService _fileDialogService;
    private readonly IRepository<FinancialTransactionEntity> _financialTransactionsRepository;
    private readonly IRepository<ImportedFilesMetadataEntity> _importedFilesMetadataRepository;

    private ImportedFilesMetadataEntity? _importingFileMetadata = null;


    [ObservableProperty] private ObservableCollection<FinancialTransactionModel> _financialTransactions = [];

    [ObservableProperty] private FlatTreeDataGridSource<FinancialTransactionModel> _flatTreeDataGridSource;

    [ObservableProperty] private bool _isError;

    [ObservableProperty] private bool _isImporting;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private int _progressBarValue;

    [ObservableProperty] private string? _statusMessage;
    
    [ObservableProperty] private int _totalLines;
    
    public MainWindowViewModel()
    {
        // Default constructor for XAML
        throw new NotImplementedException();
    }
    

    public MainWindowViewModel(IRepository<FinancialTransactionEntity> financialTransactionsRepository,
        IRepository<ImportedFilesMetadataEntity> importedFilesMetadataRepository,
        IFileDialogService fileDialogService, ICsvImportService csvImportService)
    {
        _importedFilesMetadataRepository = importedFilesMetadataRepository;
        // Импортируем зависимости
        _csvImportService = csvImportService;
        _financialTransactionsRepository = financialTransactionsRepository;
        _fileDialogService = fileDialogService;

        // Формируем FlatTreeDataGridSource
        FlatTreeDataGridSource = new FlatTreeDataGridSource<FinancialTransactionModel>(_financialTransactions)
        {
            Columns =
            {
                new TextColumn<FinancialTransactionModel, DateTime>("Дата",
                    f => f.Date),
                new TextColumn<FinancialTransactionModel, string>("Категория",
                    f => f.Category),
                new TextColumn<FinancialTransactionModel, string>("Имя",
                    f => f.Name),
                new TextColumn<FinancialTransactionModel, decimal>("Стоимость",
                    f => f.Amount, options: new TextColumnOptions<FinancialTransactionModel>
                    {
                        StringFormat = "{0:F2} \u20bd"
                    })
            }
        };
        
        // Загружаем записи из БД.
        LoadDataAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                IsError = true;
                IsLoading = false;
                StatusMessage = "Ошибка загрузки записей, перезагрузите программу или обратитесь к администратору!";
                Debug.WriteLine(task.Exception);
            }
        });
    }


    [RelayCommand]
    public async Task OpenFile()
    {
        var file = await _fileDialogService.OpenFileAsync("*.csv", "*.txt");

        if (file != null)
            await ImportCsvAsync(file).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    IsError = true;
                    IsImporting = false;
                    StatusMessage = "Во время импорта произошла ошибка, проверьте импортируемые данные!";

                    if (_importingFileMetadata != null)
                    {
                        _importingFileMetadata.IsFinishedSuccess = false;
                        _importingFileMetadata.ErrorMessage = task.Exception.Message;
                        _importedFilesMetadataRepository.UpdateAsync(_importingFileMetadata);
                        _importingFileMetadata = null;
                    }

                    Debug.WriteLine(task.Exception);
                }
            });
    }


    private async Task LoadDataAsync()
    {
        IsError = false;
        IsLoading = true;
        StatusMessage = "Загрузка записей...";

        // Переводим таблицу в IAsyncEnumirable с ленивой подгрузкой.
        var financialTransactionEntities
            = _financialTransactionsRepository
                .AsLazyAsyncEnumerable();

        var totalRecords = await _financialTransactionsRepository.TotalCount();
        
        TotalLines = totalRecords;
        
        var count = 0;

        await foreach (var entity in financialTransactionEntities)
        {
            // Добавляем каждый элемент в таблицу.
            FinancialTransactions.Add(new FinancialTransactionModel
            {
                Id = entity.Id,
                Date = entity.Date,
                Name = entity.Name,
                Category = entity.Category,
                Amount = entity.Amount
            });

            ProgressBarValue = (int)((double)count / totalRecords * 100);

            count++;
            
            if (count % 100 == 0)
                StatusMessage = $"Загружено записей: {count}";
        }
        
        StatusMessage = count > 0 
            ? StatusMessage = $"Загружено записей: {count}"
            : "Записи не обнаружены, импортируйте записи!";
        

        // Очищаем память.
        GC.Collect();
        
        IsLoading = false;
    }


    public async Task ImportCsvAsync(string filePath)
    {
        IsError = false;
        IsImporting = true;

        long fileSize = new FileInfo(filePath).Length;
        
        _importingFileMetadata = new ImportedFilesMetadataEntity()
        {
            FilePatch = filePath, 
            FileSize = fileSize
        };

        await _importedFilesMetadataRepository.AddAsync(_importingFileMetadata);

        var count = 0;
        
        // Получаем IAsyncEnumerable для сsv записей
        IAsyncEnumerable<FinancialTransactionCsvModel> records =
            _csvImportService.ReadFinancialTransactionsAsync(filePath);

        await foreach (var record in records)
        {
            // Добавление записи в БД.
            var entity = new FinancialTransactionEntity
            {
                Date = record.Date,
                Category = record.Category,
                Name = record.Name,
                Amount = record.Amount
            };

            var added = await _financialTransactionsRepository.AddAsync(entity);
            count += added;

            // Обновление коллекции для UI.
            FinancialTransactions.Add(new FinancialTransactionModel
            {
                Id = entity.Id,
                Date = record.Date,
                Category = record.Category,
                Name = record.Name,
                Amount = record.Amount
            });

            if (count % 100 == 0)
                StatusMessage = $"Импортировано записей: {count}";
        }
        
        IsImporting = false;
        
        _importingFileMetadata.LinesCount = count;
        _importingFileMetadata.IsFinishedSuccess = true;
        await _importedFilesMetadataRepository.UpdateAsync(_importingFileMetadata);
        _importingFileMetadata = null;
        
        TotalLines += count;
        
        StatusMessage = $"Записи импортированы! Количество строк: {count}, объем данных: {SizeConverter.Convert(fileSize)}";
    }
}