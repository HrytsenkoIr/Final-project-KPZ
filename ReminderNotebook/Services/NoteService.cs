using ReminderNotebook.Factories;
using ReminderNotebook.Models;
using ReminderNotebook.Repositories.Interfaces;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IReminderRepository _reminderRepository;

    public NoteService(INoteRepository noteRepository, IReminderRepository reminderRepository)
    {
        _noteRepository = noteRepository;
        _reminderRepository = reminderRepository;
    }

    public IEnumerable<Note> GetAllNotes()
    {
        var notes = _noteRepository.GetAll().ToList();
        LoadRemindersForNotes(notes);
        return notes;
    }

    public IEnumerable<Note> GetNotesByCategory(int categoryId)
    {
        var notes = _noteRepository.GetByCategory(categoryId).ToList();
        LoadRemindersForNotes(notes);
        return notes;
    }

    public IEnumerable<Note> SearchNotes(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAllNotes();

        var notes = _noteRepository.Search(query).ToList();
        LoadRemindersForNotes(notes);
        return notes;
    }

    public Note? GetNoteById(int id)
    {
        var note = _noteRepository.GetById(id);
        if (note is null) return null;

        note.Reminders = _reminderRepository.GetByNoteId(id).ToList();
        return note;
    }

    public Note CreateNote(string title, string content, int? categoryId = null)
    {
        var note = NoteFactory.Create(title, content, categoryId);
        note.Id = _noteRepository.Add(note);
        return note;
    }

    public void UpdateNote(Note note)
    {
        note.UpdatedAt = DateTime.Now;
        _noteRepository.Update(note);
    }

    public void ArchiveNote(int id) =>
        _noteRepository.ChangeStatus(id, NoteStatus.Archived);

    public void DeleteNote(int id) =>
        _noteRepository.ChangeStatus(id, NoteStatus.Deleted);

    private void LoadRemindersForNotes(IEnumerable<Note> notes)
    {
        foreach (var note in notes)
            note.Reminders = _reminderRepository.GetByNoteId(note.Id).ToList();
    }
}