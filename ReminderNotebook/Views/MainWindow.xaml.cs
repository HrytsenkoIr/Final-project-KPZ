using System.Windows;
using System.Windows.Input;
using ReminderNotebook.Services.Interfaces;
using ReminderNotebook.ViewModels;

namespace ReminderNotebook.Views;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly INoteService _noteService;
    private readonly ICategoryService _categoryService;
    private readonly IReminderService _reminderService;

    public MainWindow(
        MainViewModel viewModel,
        INoteService noteService,
        ICategoryService categoryService,
        IReminderService reminderService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _noteService = noteService;
        _categoryService = categoryService;
        _reminderService = reminderService;

        DataContext = _viewModel;
    }

    private void OnAddNoteClick(object sender, RoutedEventArgs e)
    {
        var window = new NoteEditWindow(_noteService, _categoryService, _reminderService, noteId: null);
        window.Owner = this;

        if (window.ShowDialog() == true)
            _viewModel.Refresh();
    }

    private void OnEditNoteClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: NoteViewModel noteVm }) return;
        OpenNoteEditWindow(noteVm.Id);
    }

    private void OnNoteCardClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: NoteViewModel noteVm }) return;
        OpenNoteEditWindow(noteVm.Id);
    }

    private void OnArchiveNoteClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: NoteViewModel noteVm }) return;
        _viewModel.ArchiveNoteCommand.Execute(noteVm);
    }

    private void OnDeleteNoteClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: NoteViewModel noteVm }) return;
        _viewModel.DeleteNoteCommand.Execute(noteVm);
    }

    private void OnManageCategoriesClick(object sender, RoutedEventArgs e)
    {
        var window = new CategoryManagerWindow(_categoryService);
        window.Owner = this;
        window.ShowDialog();
        _viewModel.Refresh();
    }

    private void OnShowAllNotesClick(object sender, RoutedEventArgs e)
    {
        _viewModel.SelectedCategoryId = null;
        _viewModel.SearchQuery = string.Empty;
    }

    private void OnCategoryFilterClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: int categoryId }) return;
        _viewModel.SelectedCategoryId = categoryId;
    }

    private void OpenNoteEditWindow(int noteId)
    {
        var window = new NoteEditWindow(_noteService, _categoryService, _reminderService, noteId);
        window.Owner = this;

        if (window.ShowDialog() == true)
            _viewModel.Refresh();
    }
}