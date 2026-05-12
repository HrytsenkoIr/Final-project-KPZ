namespace ReminderNotebook.Models;

public class Reminder
{
    public int Id { get; set; }
    public int NoteId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime TriggerTime { get; set; }
    public bool IsTriggered { get; set; } = false;
    public bool IsRepeating { get; set; } = false;
    public RepeatInterval RepeatInterval { get; set; } = RepeatInterval.None;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsOverdue => !IsTriggered && TriggerTime < DateTime.Now;
    public bool IsPending => !IsTriggered && TriggerTime >= DateTime.Now;
}

public enum RepeatInterval
{
    None,
    Daily,
    Weekly,
    Monthly
}