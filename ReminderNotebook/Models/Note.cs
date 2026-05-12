namespace ReminderNotebook.Models;

public class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public NoteStatus Status { get; set; } = NoteStatus.Active;
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public List<Reminder> Reminders { get; set; } = new();

    public bool HasActiveReminders =>
        Reminders.Any(r => !r.IsTriggered && r.TriggerTime > DateTime.Now);
}