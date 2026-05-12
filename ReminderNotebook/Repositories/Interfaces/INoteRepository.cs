using ReminderNotebook.Models;

namespace ReminderNotebook.Repositories.Interfaces;

public interface INoteRepository
{
    IEnumerable<Note> GetAll();
    IEnumerable<Note> GetByCategory(int categoryId);
    IEnumerable<Note> Search(string query);
    Note? GetById(int id);
    int Add(Note note);
    void Update(Note note);
    void Delete(int id);
    void ChangeStatus(int id, NoteStatus status);
}