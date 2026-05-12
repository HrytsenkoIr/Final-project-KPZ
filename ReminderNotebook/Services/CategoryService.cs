using ReminderNotebook.Models;
using ReminderNotebook.Repositories.Interfaces;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public IEnumerable<Category> GetAllCategories() =>
        _categoryRepository.GetAll();

    public Category? GetCategoryById(int id) =>
        _categoryRepository.GetById(id);

    public Category CreateCategory(string name, string colorHex)
    {
        var category = new Category
        {
            Name = name,
            ColorHex = colorHex,
            CreatedAt = DateTime.Now
        };

        category.Id = _categoryRepository.Add(category);
        return category;
    }

    public void UpdateCategory(Category category) =>
        _categoryRepository.Update(category);

    public void DeleteCategory(int id) =>
        _categoryRepository.Delete(id);
}