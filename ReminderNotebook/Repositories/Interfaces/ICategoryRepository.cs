using ReminderNotebook.Models;

namespace ReminderNotebook.Repositories.Interfaces;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
    Category? GetById(int id);
    int Add(Category category);
    void Update(Category category);
    void Delete(int id);
}