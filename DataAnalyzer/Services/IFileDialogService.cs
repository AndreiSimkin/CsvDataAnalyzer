using System.Threading.Tasks;

namespace DataAnalyzer.Services;

/// <summary>
/// Сервис показа диалоговых окон.
/// </summary>
public interface IFileDialogService
{
    Task<string?> OpenFileAsync(params string[] patterns);
}