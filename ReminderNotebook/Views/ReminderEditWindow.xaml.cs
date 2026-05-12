using System.Windows;
using ReminderNotebook.Models;
using ReminderNotebook.ViewModels;

namespace ReminderNotebook.Views;

public partial class ReminderEditWindow : Window
{
    private readonly int _noteId;
    public ReminderViewModel? Result { get; private set; }

    public ReminderEditWindow(int noteId, ReminderViewModel? reminderVm)
    {
        InitializeComponent();

        _noteId = noteId;

        InitializeIntervalComboBox();
        PopulateFields(reminderVm);
    }

    private void InitializeIntervalComboBox()
    {
        RepeatIntervalComboBox.ItemsSource = Enum.GetValues<RepeatInterval>()
            .Where(i => i != RepeatInterval.None)
            .Select(i => new { Value = i, Display = GetIntervalDisplay(i) })
            .ToList();

        RepeatIntervalComboBox.DisplayMemberPath = "Display";
        RepeatIntervalComboBox.SelectedValuePath = "Value";
        RepeatIntervalComboBox.SelectedIndex = 0;
    }

    private void PopulateFields(ReminderViewModel? reminderVm)
    {
        if (reminderVm is null)
        {
            TriggerDatePicker.SelectedDate = DateTime.Now.AddDays(1);
            TriggerTimeTextBox.Text = "09:00";
            return;
        }

        MessageTextBox.Text = reminderVm.Message;
        TriggerDatePicker.SelectedDate = reminderVm.TriggerDate;
        TriggerTimeTextBox.Text = reminderVm.TriggerTime.ToString(@"hh\:mm");
        IsRepeatingCheckBox.IsChecked = reminderVm.IsRepeating;

        if (reminderVm.IsRepeating)
        {
            IntervalPanel.Visibility = Visibility.Visible;
            RepeatIntervalComboBox.SelectedValue = reminderVm.RepeatInterval;
        }
    }

    private void OnIsRepeatingChanged(object sender, RoutedEventArgs e)
    {
        IntervalPanel.Visibility = IsRepeatingCheckBox.IsChecked == true
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void OnSaveClick(object sender, RoutedEventArgs e)
    {
        if (!ValidateInput()) return;

        var time = TimeSpan.Parse(TriggerTimeTextBox.Text);
        var isRepeating = IsRepeatingCheckBox.IsChecked == true;
        var interval = isRepeating
            ? (RepeatInterval)(RepeatIntervalComboBox.SelectedValue ?? RepeatInterval.Daily)
            : RepeatInterval.None;

        Result = new ReminderViewModel
        {
            NoteId = _noteId,
            Message = MessageTextBox.Text.Trim(),
            TriggerDate = TriggerDatePicker.SelectedDate!.Value,
            TriggerTime = time,
            IsRepeating = isRepeating,
            RepeatInterval = interval
        };

        DialogResult = true;
        Close();
    }

    private void OnCancelClick(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(MessageTextBox.Text))
        {
            ShowWarning("Введіть повідомлення нагадування.");
            MessageTextBox.Focus();
            return false;
        }

        if (TriggerDatePicker.SelectedDate is null)
        {
            ShowWarning("Оберіть дату нагадування.");
            return false;
        }

        if (!TimeSpan.TryParse(TriggerTimeTextBox.Text, out var time))
        {
            ShowWarning("Введіть час у форматі ГГ:ХХ (наприклад, 09:00).");
            TriggerTimeTextBox.Focus();
            return false;
        }

        var triggerDateTime = TriggerDatePicker.SelectedDate.Value.Date + time;
        if (triggerDateTime <= DateTime.Now)
        {
            ShowWarning("Дата та час нагадування повинні бути в майбутньому.");
            return false;
        }

        return true;
    }

    private static void ShowWarning(string message) =>
        MessageBox.Show(message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Warning);

    private static string GetIntervalDisplay(RepeatInterval interval) => interval switch
    {
        RepeatInterval.Daily => "Щодня",
        RepeatInterval.Weekly => "Щотижня",
        RepeatInterval.Monthly => "Щомісяця",
        _ => interval.ToString()
    };
}