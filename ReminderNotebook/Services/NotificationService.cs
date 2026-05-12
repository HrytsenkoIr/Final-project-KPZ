using ReminderNotebook.Models;
using ReminderNotebook.Observers;
using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.Services;

public class NotificationService : INotificationService, IReminderObserver
{
    private readonly IReminderService _reminderService;
    private readonly ReminderNotifier _notifier;
    private readonly PeriodicTimer _timer;
    private CancellationTokenSource? _cancellationTokenSource;

    public event EventHandler<Reminder>? ReminderTriggered;

    public NotificationService(IReminderService reminderService, ReminderNotifier notifier)
    {
        _reminderService = reminderService;
        _notifier = notifier;
        _timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        _notifier.Subscribe(this);
    }

    public void Start()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _ = RunCheckLoopAsync(_cancellationTokenSource.Token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _notifier.Unsubscribe(this);
    }

    public void OnReminderTriggered(Reminder reminder) =>
        ReminderTriggered?.Invoke(this, reminder);

    private async Task RunCheckLoopAsync(CancellationToken cancellationToken)
    {
        while (await _timer.WaitForNextTickAsync(cancellationToken))
        {
            try
            {
                _reminderService.ProcessPendingReminders();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error processing reminders: {ex.Message}");
            }
        }
    }
}