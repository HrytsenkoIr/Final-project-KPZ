using System.Windows;
using ReminderNotebook.Data;
using ReminderNotebook.Observers;
using ReminderNotebook.Repositories;
using ReminderNotebook.Services;
using ReminderNotebook.ViewModels;
using ReminderNotebook.Views;

namespace ReminderNotebook;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var dbContext = new DatabaseContext();

        var categoryRepository = new CategoryRepository(dbContext);
        var noteRepository = new NoteRepository(dbContext);
        var reminderRepository = new ReminderRepository(dbContext);

        var notifier = new ReminderNotifier();

        var categoryService = new CategoryService(categoryRepository);
        var reminderService = new ReminderService(reminderRepository, notifier);
        var noteService = new NoteService(noteRepository, reminderRepository);
        var notificationService = new NotificationService(reminderService, notifier);

        var mainViewModel = new MainViewModel(noteService, categoryService, notificationService);

        var mainWindow = new MainWindow(mainViewModel, noteService, categoryService, reminderService);
        mainWindow.Show();
    }
}