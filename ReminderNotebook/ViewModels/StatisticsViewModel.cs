using ReminderNotebook.Services.Interfaces;

namespace ReminderNotebook.ViewModels;

public class StatisticsViewModel : BaseViewModel
{
    public int TotalCount { get; }
    public int CompletedCount { get; }
    public int PendingCount { get; }
    public double CompletionPercentage { get; }

    public StatisticsViewModel(IStatisticsService statisticsService)
    {
        var stats = statisticsService.GetStatistics();
        
        TotalCount = stats.Total;
        CompletedCount = stats.Completed;
        PendingCount = stats.Pending;
        CompletionPercentage = stats.Percentage;
    }
}
