using ReminderNotebook.Models;

namespace ReminderNotebook.Observers;

public class ReminderNotifier
{
    private readonly List<IReminderObserver> _observers = new();

    public void Subscribe(IReminderObserver observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
    }

    public void Unsubscribe(IReminderObserver observer) =>
        _observers.Remove(observer);

    public void Notify(Reminder reminder)
    {
        foreach (var observer in _observers.ToList())
            observer.OnReminderTriggered(reminder);
    }
}