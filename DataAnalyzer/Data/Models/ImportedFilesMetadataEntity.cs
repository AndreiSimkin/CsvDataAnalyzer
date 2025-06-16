using SQLite;

namespace DataAnalyzer.Data.Models;

public class ImportedFilesMetadataEntity
{
    /// <summary>
    /// Id записи.
    /// </summary>
    [PrimaryKey]
    [AutoIncrement] 
    public int Id { get; set; }
    
    /// <summary>
    /// Импорт завершен успешно?
    /// </summary>
    public bool IsFinishedSuccess { get; set; }
    
    /// <summary>
    /// Размер файла.
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// Путь до файла.
    /// </summary>
    public string? FilePatch { get; set; }
    
    /// <summary>
    /// Количество строк, 0 если IsFinishedSuccess = true
    /// </summary>
    public int LinesCount { get; set; }
    
    /// <summary>
    /// Текст ошибки, null если IsFinishedSuccess = true.
    /// </summary>
    public string? ErrorMessage { get; set; }
}