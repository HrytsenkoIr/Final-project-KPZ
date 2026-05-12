using System.Collections.ObjectModel;
using ReminderNotebook.Models;

namespace ReminderNotebook.ViewModels;

public class NoteViewModel : BaseViewModel
{
    private int _id;
    private string _title = string.Empty;
    private string _content = string.Empty;
    private NoteStatus _status;
    private int? _categoryId;
    private string? _categoryName;
    private string _categoryColorHex = "#607D8B";
    private DateTime _createdAt;
    private DateTime _updatedAt;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => SetField(ref _title, value);
    }

    public string Content
    {
        get => _content;
        set => SetField(ref _content, value);
    }

    public NoteStatus Status
    {
        get => _status;
        set => SetField(ref _status, value);
    }

    public int? CategoryId
    {
        get => _categoryId;
        set => SetField(ref _categoryId, value);
    }

    public string? CategoryName
    {
        get => _categoryName;
        set => SetField(ref _categoryName, value);
    }

    public string CategoryColorHex
    {
        get => _categoryColorHex;
        set => SetField(ref _categoryColorHex, value);
    }

    public DateTime CreatedAt
    {
        get => _createdAt;
        set => SetField(ref _createdAt, value);
    }

    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set => SetField(ref _updatedAt, value);
    }

    public ObservableCollection<ReminderViewModel> Reminders { get; } = new();

    public bool HasActiveReminders =>
        Reminders.Any(r => !r.IsTriggered && r.TriggerDateTime > DateTime.Now);

    public string ContentPreview =>
        Content.Length > 100 ? Content[..97] + "..." : Content;

    public static NoteViewModel FromModel(Note note)
    {
        var vm = new NoteViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Status = note.Status,
            CategoryId = note.CategoryId,
            CategoryName = note.Category?.Name,
            CategoryColorHex = note.Category?.ColorHex ?? "#607D8B",
            CreatedAt = note.CreatedAt,
            UpdatedAt = note.UpdatedAt
        };

        foreach (var reminder in note.Reminders)
            vm.Reminders.Add(ReminderViewModel.FromModel(reminder));

        return vm;
    }

    public Note ToModel() => new()
    {
        Id = Id,
        Title = Title,
        Content = Content,
        Status = Status,
        CategoryId = CategoryId,
        UpdatedAt = DateTime.Now
    };
}