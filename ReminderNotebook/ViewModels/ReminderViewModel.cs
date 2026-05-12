using ReminderNotebook.Models;

namespace ReminderNotebook.ViewModels;

public class ReminderViewModel : BaseViewModel
{
    private int _id;
    private int _noteId;
    private string _message = string.Empty;
    private DateTime _triggerDate = DateTime.Now.Date.AddDays(1);
    private TimeSpan _triggerTime = TimeSpan.FromHours(9);
    private bool _isRepeating;
    private RepeatInterval _repeatInterval = RepeatInterval.None;
    private bool _isTriggered;

    public int Id
    {
        get => _id;
        set => SetField(ref _id, value);
    }

    public int NoteId
    {
        get => _noteId;
        set => SetField(ref _noteId, value);
    }

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    public DateTime TriggerDate
    {
        get => _triggerDate;
        set
        {
            if (SetField(ref _triggerDate, value))
            {
                OnPropertyChanged(nameof(TriggerDateTime));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public TimeSpan TriggerTime
    {
        get => _triggerTime;
        set
        {
            if (SetField(ref _triggerTime, value))
            {
                OnPropertyChanged(nameof(TriggerDateTime));
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public DateTime TriggerDateTime => TriggerDate.Date + TriggerTime;

    public bool IsRepeating
    {
        get => _isRepeating;
        set => SetField(ref _isRepeating, value);
    }

    public RepeatInterval RepeatInterval
    {
        get => _repeatInterval;
        set => SetField(ref _repeatInterval, value);
    }

    public bool IsTriggered
    {
        get => _isTriggered;
        set
        {
            if (SetField(ref _isTriggered, value))
            {
                OnPropertyChanged(nameof(StatusText));
            }
        }
    }

    public string StatusText => IsTriggered ? "Спрацювало" :
                                TriggerDateTime < DateTime.Now ? "Прострочено" : "Активне";

    public static ReminderViewModel FromModel(Reminder reminder) => new()
    {
        Id = reminder.Id,
        NoteId = reminder.NoteId,
        Message = reminder.Message,
        TriggerDate = reminder.TriggerTime.Date,
        TriggerTime = reminder.TriggerTime.TimeOfDay,
        IsRepeating = reminder.IsRepeating,
        RepeatInterval = reminder.RepeatInterval,
        IsTriggered = reminder.IsTriggered
    };

    public Reminder ToModel() => new()
    {
        Id = Id,
        NoteId = NoteId,
        Message = Message,
        TriggerTime = TriggerDateTime,
        IsRepeating = IsRepeating,
        RepeatInterval = RepeatInterval,
        IsTriggered = IsTriggered
    };
}