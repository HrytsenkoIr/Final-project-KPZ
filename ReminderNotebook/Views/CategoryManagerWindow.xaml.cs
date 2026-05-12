using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ReminderNotebook.Services.Interfaces;
using ReminderNotebook.ViewModels;

namespace ReminderNotebook.Views;

public partial class CategoryManagerWindow : Window
{
    private readonly ICategoryService _categoryService;
    private string _selectedColorHex = "#607D8B";

    private static readonly string[] PredefinedColors =
    {
        "#F44336", "#E91E63", "#9C27B0", "#673AB7",
        "#3F51B5", "#2196F3", "#03A9F4", "#00BCD4",
        "#009688", "#4CAF50", "#8BC34A", "#CDDC39",
        "#FFC107", "#FF9800", "#FF5722", "#607D8B"
    };

    public CategoryManagerWindow(ICategoryService categoryService)
    {
        InitializeComponent();
        _categoryService = categoryService;
        LoadCategories();
    }

    private void LoadCategories()
    {
        var categories = _categoryService.GetAllCategories()
            .Select(CategoryViewModel.FromModel)
            .ToList();

        CategoriesItemsControl.ItemsSource = categories;
    }

    private void OnAddCategoryClick(object sender, RoutedEventArgs e)
    {
        var name = NewCategoryNameTextBox.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            MessageBox.Show("Введіть назву категорії.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);
            NewCategoryNameTextBox.Focus();
            return;
        }

        _categoryService.CreateCategory(name, _selectedColorHex);
        NewCategoryNameTextBox.Clear();
        LoadCategories();
    }

    private void OnCategoryNameLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is not TextBox { Tag: CategoryViewModel vm }) return;
        _categoryService.UpdateCategory(vm.ToModel());
    }

    private void OnEditCategoryColorClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: CategoryViewModel vm }) return;

        var picker = new ColorPickerWindow(PredefinedColors, vm.ColorHex);
        picker.Owner = this;

        if (picker.ShowDialog() != true) return;

        vm.ColorHex = picker.SelectedColorHex;
        _categoryService.UpdateCategory(vm.ToModel());
        LoadCategories();
    }

    private void OnSaveCategoryClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: CategoryViewModel vm }) return;

        _categoryService.UpdateCategory(vm.ToModel());
        MessageBox.Show("Збережено.", "ReminderNotebook", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void OnDeleteCategoryClick(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: CategoryViewModel vm }) return;

        var result = MessageBox.Show(
            $"Видалити категорію \"{vm.Name}\"?\nНотатки залишаться без категорії.",
            "Підтвердження",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes) return;

        _categoryService.DeleteCategory(vm.Id);
        LoadCategories();
    }

    private void OnColorPickerClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        var picker = new ColorPickerWindow(PredefinedColors, _selectedColorHex);
        picker.Owner = this;

        if (picker.ShowDialog() != true) return;

        _selectedColorHex = picker.SelectedColorHex;
        ColorPreviewBorder.Background = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString(_selectedColorHex));
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}