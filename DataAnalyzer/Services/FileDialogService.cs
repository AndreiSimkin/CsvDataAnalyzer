using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace DataAnalyzer.Services;

/// <summary>
/// Сервис показа диалоговых окон.
/// </summary>
public class FileDialogService : IFileDialogService
{
    public async Task<string?> OpenFileAsync(params string[] patterns)
    {
        var window = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (window == null)
            return null;

        var result = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите файл...",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType(string.Join(", ", patterns))
                {
                    Patterns = patterns
                }
            ]
        });

        return result.FirstOrDefault()?.TryGetLocalPath();
    }
}