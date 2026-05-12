using ReminderNotebook.Models;

namespace ReminderNotebook.Observers;

public interface IReminderObserver
{
    void OnReminderTriggered(Reminder reminder);
}