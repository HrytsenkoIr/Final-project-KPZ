using ReminderNotebook.Models;

namespace ReminderNotebook.Services.Interfaces;

public interface ICategoryService
{
    IEnumerable<Category> GetAllCategories();
    Category? GetCategoryById(int id);
    Category CreateCategory(string name, string colorHex);
    void UpdateCategory(Category category);
    void DeleteCategory(int id);
}