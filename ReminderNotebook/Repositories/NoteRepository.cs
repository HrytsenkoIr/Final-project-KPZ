using Microsoft.Data.Sqlite;
using ReminderNotebook.Data;
using ReminderNotebook.Models;
using ReminderNotebook.Repositories.Interfaces;

namespace ReminderNotebook.Repositories;

public class NoteRepository : INoteRepository
{
    private readonly DatabaseContext _context;

    public NoteRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<Note> GetAll()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery("WHERE n.Status != 'Deleted' ORDER BY n.UpdatedAt DESC");

        return ReadNotes(command);
    }

    public IEnumerable<Note> GetByCategory(int categoryId)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery("WHERE n.CategoryId = @CategoryId AND n.Status != 'Deleted' ORDER BY n.UpdatedAt DESC");
        command.Parameters.AddWithValue("@CategoryId", categoryId);

        return ReadNotes(command);
    }

    public IEnumerable<Note> Search(string query)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery(
            "WHERE n.Status != 'Deleted' AND (n.Title LIKE @Query OR n.Content LIKE @Query) ORDER BY n.UpdatedAt DESC");
        command.Parameters.AddWithValue("@Query", $"%{query}%");

        return ReadNotes(command);
    }

    public Note? GetById(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = BuildSelectQuery("WHERE n.Id = @Id");
        command.Parameters.AddWithValue("@Id", id);

        return ReadNotes(command).FirstOrDefault();
    }

    public int Add(Note note)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Notes (Title, Content, Status, CategoryId, CreatedAt, UpdatedAt)
            VALUES (@Title, @Content, @Status, @CategoryId, @CreatedAt, @UpdatedAt);
            SELECT last_insert_rowid();
        """;

        AddNoteParameters(command, note);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Note note)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Notes
            SET Title = @Title, Content = @Content, Status = @Status,
                CategoryId = @CategoryId, UpdatedAt = @UpdatedAt
            WHERE Id = @Id
        """;

        AddNoteParameters(command, note);
        command.Parameters.AddWithValue("@Id", note.Id);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Notes WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    public void ChangeStatus(int id, NoteStatus status)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "UPDATE Notes SET Status = @Status WHERE Id = @Id";
        command.Parameters.AddWithValue("@Status", status.ToString());
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    private static string BuildSelectQuery(string whereClause) => $"""
        SELECT n.Id, n.Title, n.Content, n.Status, n.CategoryId,
               n.CreatedAt, n.UpdatedAt,
               c.Id, c.Name, c.ColorHex, c.CreatedAt
        FROM Notes n
        LEFT JOIN Categories c ON n.CategoryId = c.Id
        {whereClause}
    """;

    private static IEnumerable<Note> ReadNotes(SqliteCommand command)
    {
        using var reader = command.ExecuteReader();
        var notes = new List<Note>();

        while (reader.Read())
            notes.Add(MapNote(reader));

        return notes;
    }

    private static Note MapNote(SqliteDataReader reader)
    {
        var note = new Note
        {
            Id = reader.GetInt32(0),
            Title = reader.GetString(1),
            Content = reader.GetString(2),
            Status = Enum.Parse<NoteStatus>(reader.GetString(3)),
            CategoryId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
            CreatedAt = DateTime.Parse(reader.GetString(5)),
            UpdatedAt = DateTime.Parse(reader.GetString(6))
        };

        if (!reader.IsDBNull(7))
        {
            note.Category = new Category
            {
                Id = reader.GetInt32(7),
                Name = reader.GetString(8),
                ColorHex = reader.GetString(9),
                CreatedAt = DateTime.Parse(reader.GetString(10))
            };
        }

        return note;
    }

    private static void AddNoteParameters(SqliteCommand command, Note note)
    {
        command.Parameters.AddWithValue("@Title", note.Title);
        command.Parameters.AddWithValue("@Content", note.Content);
        command.Parameters.AddWithValue("@Status", note.Status.ToString());
        command.Parameters.AddWithValue("@CategoryId", (object?)note.CategoryId ?? DBNull.Value);
        command.Parameters.AddWithValue("@CreatedAt", note.CreatedAt.ToString("o"));
        command.Parameters.AddWithValue("@UpdatedAt", note.UpdatedAt.ToString("o"));
    }
}