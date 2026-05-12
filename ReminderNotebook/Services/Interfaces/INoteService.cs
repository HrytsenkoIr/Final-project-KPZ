using ReminderNotebook.Models;

namespace ReminderNotebook.Services.Interfaces;

public interface INoteService
{
    IEnumerable<Note> GetAllNotes();
    IEnumerable<Note> GetNotesByCategory(int categoryId);
    IEnumerable<Note> SearchNotes(string query);
    Note? GetNoteById(int id);
    Note CreateNote(string title, string content, int? categoryId = null);
    void UpdateNote(Note note);
    void ArchiveNote(int id);
    void DeleteNote(int id);
}