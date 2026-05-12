using Microsoft.Data.Sqlite;
using System.IO;

namespace ReminderNotebook.Data;

public class DatabaseContext
{
    private const string DatabaseFileName = "notebook.db";

    private readonly string _connectionString;

    public DatabaseContext()
    {
        var appDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ReminderNotebook");

        Directory.CreateDirectory(appDataPath);

        var dbPath = Path.Combine(appDataPath, DatabaseFileName);
        _connectionString = $"Data Source={dbPath}";

        InitializeDatabase();
    }

    public SqliteConnection CreateConnection() =>
        new SqliteConnection(_connectionString);

    private void InitializeDatabase()
    {
        using var connection = CreateConnection();
        connection.Open();
        CreateCategoriesTable(connection);
        CreateNotesTable(connection);
        CreateRemindersTable(connection);
    }

    private static void CreateCategoriesTable(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Categories (
                Id       INTEGER PRIMARY KEY AUTOINCREMENT,
                Name     TEXT    NOT NULL,
                ColorHex TEXT    NOT NULL DEFAULT '#607D8B',
                CreatedAt TEXT   NOT NULL
            );
        """;
        ExecuteNonQuery(connection, sql);
    }

    private static void CreateNotesTable(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Notes (
                Id         INTEGER PRIMARY KEY AUTOINCREMENT,
                Title      TEXT    NOT NULL,
                Content    TEXT    NOT NULL DEFAULT '',
                Status     TEXT    NOT NULL DEFAULT 'Active',
                CategoryId INTEGER,
                CreatedAt  TEXT    NOT NULL,
                UpdatedAt  TEXT    NOT NULL,
                FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE SET NULL
            );
        """;
        ExecuteNonQuery(connection, sql);
    }

    private static void CreateRemindersTable(SqliteConnection connection)
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Reminders (
                Id             INTEGER PRIMARY KEY AUTOINCREMENT,
                NoteId         INTEGER NOT NULL,
                Message        TEXT    NOT NULL,
                TriggerTime    TEXT    NOT NULL,
                IsTriggered    INTEGER NOT NULL DEFAULT 0,
                IsRepeating    INTEGER NOT NULL DEFAULT 0,
                RepeatInterval TEXT    NOT NULL DEFAULT 'None',
                CreatedAt      TEXT    NOT NULL,
                FOREIGN KEY (NoteId) REFERENCES Notes(Id) ON DELETE CASCADE
            );
        """;
        ExecuteNonQuery(connection, sql);
    }

    private static void ExecuteNonQuery(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}