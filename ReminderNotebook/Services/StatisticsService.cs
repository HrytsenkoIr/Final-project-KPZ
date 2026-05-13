using ReminderNotebook.Models;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class StatisticsService : IStatisticsService
{
    private readonly INoteService _noteService;

    public StatisticsService(INoteService noteService)
    {
        _noteService = noteService;
    }

    public StatisticsResult GetStatistics()
    {
        var notes = _noteService.GetAllNotes().ToList();
        
        var total = notes.Count;
        if (total == 0)
            return new StatisticsResult(0, 0, 0, 0);

        var completed = notes.Count(n => n.Status == NoteStatus.Archived || 
                                        (n.Status == NoteStatus.Active && n.Reminders.Any() && n.Reminders.All(r => r.IsTriggered)));
        
        var pending = total - completed;
        var percentage = (double)completed / total * 100;

        return new StatisticsResult(total, completed, pending, percentage);
    }
}
