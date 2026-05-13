using System.Windows;
using ReminderNotebook.ViewModels;

namespace ReminderNotebook.Views;

public partial class StatisticsWindow : Window
{
    public StatisticsWindow(StatisticsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
