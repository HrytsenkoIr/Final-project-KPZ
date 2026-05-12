namespace ReminderNotebook.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#607D8B";
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public override string ToString() => Name;
}