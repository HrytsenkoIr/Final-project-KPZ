using ReminderNotebook.Helpers;
using ReminderNotebook.Models;

namespace ReminderNotebook.Factories;

public static class ReminderFactory
{
    public static Reminder Create(
        int noteId,
        string message,
        DateTime triggerTime,
        bool isRepeating = false,
        RepeatInterval interval = RepeatInterval.None)
    {
        ValidationHelper.RequirePositive(noteId, nameof(noteId));
        ValidationHelper.RequireNonEmpty(message, nameof(message));
        ValidationHelper.RequireFutureDate(triggerTime, nameof(triggerTime));

        return new Reminder
        {
            NoteId = noteId,
            Message = message.Trim(),
            TriggerTime = triggerTime,
            IsTriggered = false,
            IsRepeating = isRepeating,
            RepeatInterval = isRepeating ? interval : RepeatInterval.None,
            CreatedAt = DateTime.Now
        };
    }
}