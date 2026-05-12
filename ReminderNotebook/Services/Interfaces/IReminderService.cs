using ReminderNotebook.Models;

namespace ReminderNotebook.Services.Interfaces;

public interface IReminderService
{
    IEnumerable<Reminder> GetRemindersForNote(int noteId);
    IEnumerable<Reminder> GetPendingReminders();
    Reminder CreateReminder(int noteId, string message, DateTime triggerTime,
        bool isRepeating = false, RepeatInterval interval = RepeatInterval.None);
    void UpdateReminder(Reminder reminder);
    void DeleteReminder(int id);
    void MarkReminderAsTriggered(int id);
    void ProcessPendingReminders();
}