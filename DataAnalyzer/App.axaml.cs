using System.Linq;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using DataAnalyzer.Data.Models;
using DataAnalyzer.Data.Repositories;
using DataAnalyzer.Services;
using DataAnalyzer.ViewModels;
using DataAnalyzer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataAnalyzer;

public class App : Application
{
    private IHost? _host;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Инициализация DI
        
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(ConfigureServices)
            .Build();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();
            
            // Создание главного окна через DI
            var mainWindow = _host.Services.GetRequiredService<MainWindowView>();
            mainWindow.DataContext = _host.Services.GetRequiredService<MainWindowViewModel>();

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Register Views
        services.AddSingleton<MainWindowView>();

        // Register ViewModels
        services.AddSingleton<MainWindowViewModel>();

        // Register services
        services.AddSingleton<IFileDialogService, FileDialogService>();
        services.AddSingleton<ICsvImportService, CsvImportService>();

        // Register Repositories
        services.AddSingleton<IRepository<ImportedFilesMetadataEntity>>(
            _ => new EntitiesRepository<ImportedFilesMetadataEntity>("data.db"));
        
        services.AddSingleton<IRepository<FinancialTransactionEntity>>(
            _ => new EntitiesRepository<FinancialTransactionEntity>("data.db"));
    }
}