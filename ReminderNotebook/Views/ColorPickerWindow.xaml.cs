using System.Windows;
using System.Windows.Media;

namespace ReminderNotebook.Views;

public partial class ColorPickerWindow : Window
{
    public string SelectedColorHex { get; private set; }

    public ColorPickerWindow(IEnumerable<string> colorHexValues, string currentColorHex)
    {
        InitializeComponent();
        SelectedColorHex = currentColorHex;

        var colors = colorHexValues
            .Select(hex => (Color)ColorConverter.ConvertFromString(hex))
            .ToList();

        ColorSwatchesControl.ItemsSource = colors;
    }

    private void OnColorSelected(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: Color color }) return;

        SelectedColorHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}