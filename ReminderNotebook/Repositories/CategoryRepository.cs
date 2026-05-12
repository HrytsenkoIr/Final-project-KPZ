using Microsoft.Data.Sqlite;
using ReminderNotebook.Data;
using ReminderNotebook.Models;
using ReminderNotebook.Repositories.Interfaces;

namespace ReminderNotebook.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly DatabaseContext _context;

    public CategoryRepository(DatabaseContext context)
    {
        _context = context;
    }

    public IEnumerable<Category> GetAll()
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, ColorHex, CreatedAt FROM Categories ORDER BY Name";

        using var reader = command.ExecuteReader();
        var categories = new List<Category>();

        while (reader.Read())
            categories.Add(MapCategory(reader));

        return categories;
    }

    public Category? GetById(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, ColorHex, CreatedAt FROM Categories WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);

        using var reader = command.ExecuteReader();
        return reader.Read() ? MapCategory(reader) : null;
    }

    public int Add(Category category)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Categories (Name, ColorHex, CreatedAt)
            VALUES (@Name, @ColorHex, @CreatedAt);
            SELECT last_insert_rowid();
        """;

        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@ColorHex", category.ColorHex);
        command.Parameters.AddWithValue("@CreatedAt", category.CreatedAt.ToString("o"));

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void Update(Category category)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = """
            UPDATE Categories
            SET Name = @Name, ColorHex = @ColorHex
            WHERE Id = @Id
        """;

        command.Parameters.AddWithValue("@Name", category.Name);
        command.Parameters.AddWithValue("@ColorHex", category.ColorHex);
        command.Parameters.AddWithValue("@Id", category.Id);

        command.ExecuteNonQuery();
    }

    public void Delete(int id)
    {
        using var connection = _context.CreateConnection();
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM Categories WHERE Id = @Id";
        command.Parameters.AddWithValue("@Id", id);
        command.ExecuteNonQuery();
    }

    private static Category MapCategory(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        ColorHex = reader.GetString(2),
        CreatedAt = DateTime.Parse(reader.GetString(3))
    };
}