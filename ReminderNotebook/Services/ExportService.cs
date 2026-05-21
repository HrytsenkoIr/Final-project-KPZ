using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class ExportService : IExportService
{
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;

    public ExportService(INoteService noteService, ICategoryService categoryService)
    {
        _noteService = noteService;
        _categoryService = categoryService;
    }

    public async Task<bool> ExportToJsonAsync(string filePath)
    {
        try
        {
            var notes = _noteService.GetAllNotes().ToList();
            var categories = _categoryService.GetAllCategories().ToList();

            var exportData = new
            {
                ExportDate = DateTime.Now,
                AppVersion = "1.0.0",
                Categories = categories,
                Notes = notes
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(exportData, options);
            await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Export error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ExportToCsvAsync(string filePath)
    {
        try
        {
            var notes = _noteService.GetAllNotes().ToList();
            var csv = new StringBuilder();

            csv.AppendLine("Id;Title;Category;Status;CreatedAt;UpdatedAt;ContentPreview");

            foreach (var note in notes)
            {
                var categoryName = note.Category?.Name ?? "None";
                
                var title = EscapeCsvField(note.Title);
                var category = EscapeCsvField(categoryName);
                var content = EscapeCsvField(note.Content);

                csv.AppendLine($"{note.Id};{title};{category};{note.Status};{note.CreatedAt:o};{note.UpdatedAt:o};{content}");
            }

            await File.WriteAllTextAsync(filePath, csv.ToString(), Encoding.UTF8);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"CSV Export error: {ex.Message}");
            return false;
        }
    }

    private string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field)) return string.Empty;

        if (field.Contains(";") || field.Contains("\"") || field.Contains("\n") || field.Contains("\r"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }

        return field;
    }
}
