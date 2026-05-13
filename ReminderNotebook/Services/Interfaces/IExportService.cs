namespace ReminderNotebook.Services.Interfaces;

public interface IExportService
{

    Task<bool> ExportToJsonAsync(string filePath);
    
    Task<bool> ExportToCsvAsync(string filePath);
}
