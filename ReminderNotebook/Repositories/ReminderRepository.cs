using Microsoft.Data.Sqlite;
using ReminderNotebook.Data;
using ReminderNotebook.Models;
using ReminderNotebook.Repositories.Interfaces;

namespace ReminderNotebook.Repositories;

public class ReminderRepository : IReminderRepository
{
    private readonly DatabaseContext _context;

    public ReminderRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<Reminder> GetByNoteId(int noteId)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery("WHERE NoteId = @NoteId ORDER BY TriggerTime");
        command.Parameters.AddWithValue("@NoteId", noteId);

        return ReadReminders(command);
    }

    public IEnumerable<Reminder> GetPendingReminders()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery(
            "WHERE IsTriggered = 0 AND TriggerTime <= @Now ORDER BY TriggerTime");
        command.Parameters.AddWithValue("@Now", DateTime.Now.ToString("o"));

        return ReadReminders(command);
    }

    public Reminder? GetById(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery("WHERE Id = @Id");
        command.Parameters.AddWithValue("@Id", id);

        return ReadReminders(command).FirstOrDefault();
    }

    public int Add(Reminder reminder)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Reminders (NoteId, Message, TriggerTime, IsTriggered, IsRepeating, RepeatInterval, CreatedAt)
            VALUES (@NoteId, @Message, @TriggerTime, @IsTriggered, @IsRepeating, @RepeatInterval, @CreatedAt);
            SELECT last_insert_rowid();
        """;

        AddReminderParameters(command, reminder);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Reminder reminder)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Reminders
            SET Message = @Message, TriggerTime = @TriggerTime,
                IsTriggered = @IsTriggered, IsRepeating = @IsRepeating,
                RepeatInterval = @RepeatInterval
            WHERE Id = @Id
        """;

        AddReminderParameters(command, reminder);
        command.Parameters.AddWithValue("@Id", reminder.Id);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Reminders WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    public void MarkAsTriggered(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Reminders SET IsTriggered = 1 WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    private static string BuildSelectQuery(string whereClause) =>
        $"SELECT Id, NoteId, Message, TriggerTime, IsTriggered, IsRepeating, RepeatInterval, CreatedAt FROM Reminders {whereClause}";

    private static IEnumerable<Reminder> ReadReminders(SqliteCommand command)
    {
        using var reader = command.ExecuteReader();
        var reminders = new List<Reminder>();

        while (reader.Read())
            reminders.Add(MapReminder(reader));

        return reminders;
    }

    private static Reminder MapReminder(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        NoteId = reader.GetInt32(1),
        Message = reader.GetString(2),
        TriggerTime = DateTime.Parse(reader.GetString(3)),
        IsTriggered = reader.GetInt32(4) == 1,
        IsRepeating = reader.GetInt32(5) == 1,
        RepeatInterval = Enum.Parse<RepeatInterval>(reader.GetString(6)),
        CreatedAt = DateTime.Parse(reader.GetString(7))
    };

    private static void AddReminderParameters(SqliteCommand command, Reminder reminder)
    {
        command.Parameters.AddWithValue("@NoteId", reminder.NoteId);
        command.Parameters.AddWithValue("@Message", reminder.Message);
        command.Parameters.AddWithValue("@TriggerTime", reminder.TriggerTime.ToString("o"));
        command.Parameters.AddWithValue("@IsTriggered", reminder.IsTriggered ? 1 : 0);
        command.Parameters.AddWithValue("@IsRepeating", reminder.IsRepeating ? 1 : 0);
        command.Parameters.AddWithValue("@RepeatInterval", reminder.RepeatInterval.ToString());
        command.Parameters.AddWithValue("@CreatedAt", reminder.CreatedAt.ToString("o"));
    }
}