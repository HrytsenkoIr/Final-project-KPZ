using ReminderNotebook.Models;

namespace ReminderNotebook.Repositories.Interfaces;

public interface IReminderRepository
{
    IEnumerable<Reminder> GetByNoteId(int noteId);
    IEnumerable<Reminder> GetPendingReminders();
    Reminder? GetById(int id);
    int Add(Reminder reminder);
    void Update(Reminder reminder);
    void Delete(int id);
    void MarkAsTriggered(int id);
}