using ReminderNotebook.Models;

namespace ReminderNotebook.ViewModels;

public class CategoryViewModel : BaseViewModel
{
    private int _id;
    private string _name = string.Empty;
    private string _colorHex = "#607D8B";

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetField(ref _name, value);
    }

    public string ColorHex
    {
        get => _colorHex;
        set => SetField(ref _colorHex, value);
    }

    public static CategoryViewModel FromModel(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        ColorHex = category.ColorHex
    };

    public Category ToModel() => new()
    {
        Id = Id,
        Name = Name,
        ColorHex = ColorHex
    };
}