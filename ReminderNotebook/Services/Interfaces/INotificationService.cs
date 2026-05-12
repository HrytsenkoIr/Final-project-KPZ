using ReminderNotebook.Models;

namespace ReminderNotebook.Services.Interfaces;

public interface INotificationService
{
    void Start();
    void Stop();
    event EventHandler<Reminder> ReminderTriggered;
}