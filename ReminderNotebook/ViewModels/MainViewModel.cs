using System.Collections.ObjectModel;
using System.Windows;
using ReminderNotebook.Helpers;
using ReminderNotebook.Models;
using ReminderNotebook.Observers;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.ViewModels;

public class MainViewModel : BaseViewModel, IReminderObserver
{
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;
    private readonly INotificationService _notificationService;

    private string _searchQuery = string.Empty;
    private int? _selectedCategoryId;
    private NoteViewModel? _selectedNote;
    private bool _isLoading;

    public ObservableCollection<NoteViewModel> Notes { get; } = new();
    public ObservableCollection<CategoryViewModel> Categories { get; } = new();

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetField(ref _searchQuery, value))
                LoadNotesCommand.Execute(null);
        }
    }

    public int? SelectedCategoryId
    {
        get => _selectedCategoryId;
        set
        {
            if (SetField(ref _selectedCategoryId, value))
                LoadNotesCommand.Execute(null);
        }
    }

    public NoteViewModel? SelectedNote
    {
        get => _selectedNote;
        set => SetField(ref _selectedNote, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetField(ref _isLoading, value);
    }

    public RelayCommand LoadNotesCommand { get; }
    public RelayCommand LoadCategoriesCommand { get; }
    public RelayCommand<NoteViewModel> ArchiveNoteCommand { get; }
    public RelayCommand<NoteViewModel> DeleteNoteCommand { get; }
    public RelayCommand ClearSearchCommand { get; }
    public RelayCommand ClearCategoryFilterCommand { get; }

    public MainViewModel(
        INoteService noteService,
        ICategoryService categoryService,
        INotificationService notificationService)
    {
        _noteService = noteService;
        _categoryService = categoryService;
        _notificationService = notificationService;

        LoadNotesCommand = new RelayCommand(LoadNotes);
        LoadCategoriesCommand = new RelayCommand(LoadCategories);
        ArchiveNoteCommand = new RelayCommand<NoteViewModel>(ArchiveNote);
        DeleteNoteCommand = new RelayCommand<NoteViewModel>(ConfirmAndDeleteNote);
        ClearSearchCommand = new RelayCommand(() => SearchQuery = string.Empty);
        ClearCategoryFilterCommand = new RelayCommand(() => SelectedCategoryId = null);

        _notificationService.ReminderTriggered += OnReminderTriggeredEvent;
        _notificationService.Start();

        LoadCategories();
        LoadNotes();
    }

    public void OnReminderTriggered(Reminder reminder) =>
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(
                $"Нагадування!\n\n{reminder.Message}",
                "ReminderNotebook",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        });

    private void OnReminderTriggeredEvent(object? sender, Reminder reminder) =>
        OnReminderTriggered(reminder);

    private void LoadNotes()
    {
        IsLoading = true;
        Notes.Clear();

        try
        {
            var notes = GetFilteredNotes();

            foreach (var note in notes)
                Notes.Add(NoteViewModel.FromModel(note));
        }
        finally
        {
            IsLoading = false;
        }
    }

    private IEnumerable<Note> GetFilteredNotes()
    {
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            return _noteService.SearchNotes(SearchQuery);

        if (SelectedCategoryId.HasValue)
            return _noteService.GetNotesByCategory(SelectedCategoryId.Value);

        return _noteService.GetAllNotes();
    }

    private void LoadCategories()
    {
        Categories.Clear();
        foreach (var category in _categoryService.GetAllCategories())
            Categories.Add(CategoryViewModel.FromModel(category));
    }

    private void ArchiveNote(NoteViewModel? noteVm)
    {
        if (noteVm is null) return;
        _noteService.ArchiveNote(noteVm.Id);
        Notes.Remove(noteVm);
    }

    private void ConfirmAndDeleteNote(NoteViewModel? noteVm)
    {
        if (noteVm is null) return;

        var result = MessageBox.Show(
            $"Видалити нотатку \"{noteVm.Title}\"?",
            "Підтвердження",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        _noteService.DeleteNote(noteVm.Id);
        Notes.Remove(noteVm);
    }

    public void Refresh()
    {
        LoadCategories();
        LoadNotes();
    }
}

public class RelayCommand<T> : RelayCommand
{
    public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        : base(obj => execute(obj is T t ? t : default), obj => canExecute?.Invoke(obj is T t ? t : default) ?? true)
    {
    }
}