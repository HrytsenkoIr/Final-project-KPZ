using ReminderNotebook.Factories;
using ReminderNotebook.Models;
using ReminderNotebook.Observers;
using ReminderNotebook.Repositories.Interfaces;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class ReminderService : IReminderService
{
    private readonly IReminderRepository _reminderRepository;
    private readonly ReminderNotifier _notifier;

    public ReminderService(IReminderRepository reminderRepository, ReminderNotifier notifier)
    {
        _reminderRepository = reminderRepository;
        _notifier = notifier;
    }

    public IEnumerable<Reminder> GetRemindersForNote(int noteId) =>
        _reminderRepository.GetByNoteId(noteId);

    public IEnumerable<Reminder> GetPendingReminders() =>
        _reminderRepository.GetPendingReminders();

    public Reminder CreateReminder(int noteId, string message, DateTime triggerTime,
        bool isRepeating = false, RepeatInterval interval = RepeatInterval.None)
    {
        var reminder = ReminderFactory.Create(noteId, message, triggerTime, isRepeating, interval);
        reminder.Id = _reminderRepository.Add(reminder);
        return reminder;
    }

    public void UpdateReminder(Reminder reminder) =>
        _reminderRepository.Update(reminder);

    public void DeleteReminder(int id) =>
        _reminderRepository.Delete(id);

    public void MarkReminderAsTriggered(int id) =>
        _reminderRepository.MarkAsTriggered(id);

    public void ProcessPendingReminders()
    {
        var pendingReminders = _reminderRepository.GetPendingReminders().ToList();

        foreach (var reminder in pendingReminders)
        {
            _reminderRepository.MarkAsTriggered(reminder.Id);
            _notifier.Notify(reminder);

            if (reminder.IsRepeating)
                RescheduleReminder(reminder);
        }
    }

    private void RescheduleReminder(Reminder reminder)
    {
        var nextTrigger = CalculateNextTriggerTime(reminder.TriggerTime, reminder.RepeatInterval);

        var newReminder = ReminderFactory.Create(
            reminder.NoteId,
            reminder.Message,
            nextTrigger,
            reminder.IsRepeating,
            reminder.RepeatInterval);

        _reminderRepository.Add(newReminder);
    }

    private static DateTime CalculateNextTriggerTime(DateTime current, RepeatInterval interval) =>
        interval switch
        {
            RepeatInterval.Daily => current.AddDays(1),
            RepeatInterval.Weekly => current.AddDays(7),
            RepeatInterval.Monthly => current.AddMonths(1),
            _ => current
        };
}