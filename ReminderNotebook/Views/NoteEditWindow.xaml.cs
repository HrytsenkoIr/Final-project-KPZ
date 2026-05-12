using System.Windows;
using ReminderNotebook.Models;
using ReminderNotebook.Services.Interfaces;
using ReminderNotebook.ViewModels;

namespace ReminderNotebook.Views;

public partial class NoteEditWindow : Window
{
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;
    private readonly IReminderService _reminderService;

    private Note? _note;
    private readonly List<ReminderViewModel> _reminders = new();
    private readonly List<int> _deletedReminderIds = new();

    public NoteEditWindow(
        INoteService noteService,
        ICategoryService categoryService,
        IReminderService reminderService,
        int? noteId)
    {
        InitializeComponent();

        _noteService = noteService;
        _categoryService = categoryService;
        _reminderService = reminderService;

        LoadCategories();

        if (noteId.HasValue)
            LoadNote(noteId.Value);
    }

    private void LoadCategories()
    {
        var categories = _categoryService.GetAllCategories().ToList();

        var noneItem = new Category { Id = 0, Name = "(без категорії)" };
        categories.Insert(0, noneItem);

        CategoryComboBox.ItemsSource = categories;
        CategoryComboBox.SelectedIndex = 0;
    }

    private void LoadNote(int noteId)
    {
        _note = _noteService.GetNoteById(noteId);
        if (_note is null) return;

        Title = "Редагувати нотатку";
        TitleTextBox.Text = _note.Title;
        ContentTextBox.Text = _note.Content;

        var categories = CategoryComboBox.ItemsSource as IEnumerable<Category>;
        var selected = categories?.FirstOrDefault(c => c.Id == _note.CategoryId);
        if (selected is not null)
            CategoryComboBox.SelectedItem = selected;

        _reminders.AddRange(_note.Reminders.Select(ReminderViewModel.FromModel));
        RefreshRemindersList();
    }

    private void RefreshRemindersList()
    {
        RemindersItemsControl.ItemsSource = null;
        RemindersItemsControl.ItemsSource = _reminders;
        NoRemindersText.Visibility = _reminders.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnAddReminderClick(object sender, RoutedEventArgs e)
    {
        if (!EnsureNoteExists()) return;

        var window = new ReminderEditWindow(_note!.Id, reminderVm: null);
        window.Owner = this;

        if (window.ShowDialog() != true || window.Result is null) return;

        var created = _reminderService.CreateReminder(
            _note.Id,
            window.Result.Message,
            window.Result.TriggerDateTime,
            window.Result.IsRepeating,
            window.Result.RepeatInterval);

        _reminders.Add(ReminderViewModel.FromModel(created));
        RefreshRemindersList();
    }

    private void OnEditReminderClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: ReminderViewModel reminderVm }) return;

        var window = new ReminderEditWindow(_note?.Id ?? 0, reminderVm);
        window.Owner = this;

        if (window.ShowDialog() != true || window.Result is null) return;

        reminderVm.Message = window.Result.Message;
        reminderVm.TriggerDate = window.Result.TriggerDate;
        reminderVm.TriggerTime = window.Result.TriggerTime;
        reminderVm.IsRepeating = window.Result.IsRepeating;
        reminderVm.RepeatInterval = window.Result.RepeatInterval;
        reminderVm.IsTriggered = window.Result.IsTriggered;

        _reminderService.UpdateReminder(reminderVm.ToModel());
        RefreshRemindersList();
    }

    private void OnDeleteReminderClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: ReminderViewModel reminderVm }) return;

        if (reminderVm.Id > 0)
            _deletedReminderIds.Add(reminderVm.Id);

        _reminders.Remove(reminderVm);
        RefreshRemindersList();
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) return;

        var selectedCategory = CategoryComboBox.SelectedItem as Category;
        var categoryId = selectedCategory?.Id > 0 ? selectedCategory.Id : (int?)null;

        if (_note is null)
        {
            _note = _noteService.CreateNote(TitleTextBox.Text, ContentTextBox.Text, categoryId);
        }
        else
        {
            _note.Title = TitleTextBox.Text;
            _note.Content = ContentTextBox.Text;
            _note.CategoryId = categoryId;
            _noteService.UpdateNote(_note);
        }

        DeletePendingReminders();

        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private bool ValidateInput()
    {
        if (!string.IsNullOrWhiteSpace(TitleTextBox.Text)) return true;

        MessageBox.Show("Введіть заголовок нотатки.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
        TitleTextBox.Focus();
        return false;
    }

    private bool EnsureNoteExists()
    {
        if (_note is not null) return true;

        if (string.IsNullOrWhiteSpace(TitleTextBox.Text))
        {
            MessageBox.Show("Спочатку введіть заголовок і збережіть нотатку.",
                "Увага", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }

        var selectedCategory = CategoryComboBox.SelectedItem as Category;
        var categoryId = selectedCategory?.Id > 0 ? selectedCategory.Id : (int?)null;

        _note = _noteService.CreateNote(TitleTextBox.Text, ContentTextBox.Text, categoryId);
        return true;
    }

    private void DeletePendingReminders()
    {
        foreach (var id in _deletedReminderIds)
            _reminderService.DeleteReminder(id);
    }
}