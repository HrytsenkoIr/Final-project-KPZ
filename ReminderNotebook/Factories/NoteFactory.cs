using ReminderNotebook.Helpers;
using ReminderNotebook.Models;

namespace ReminderNotebook.Factories;

public static class NoteFactory
{
    public static Note Create(string title, string content, int? categoryId = null)
    {
        ValidationHelper.RequireNonEmpty(title, nameof(title));

        return new Note
        {
            Title = title.Trim(),
            Content = content.Trim(),
            CategoryId = categoryId,
            Status = NoteStatus.Active,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };
    }
}