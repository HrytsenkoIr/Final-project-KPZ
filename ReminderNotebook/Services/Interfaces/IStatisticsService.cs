namespace ReminderNotebook.Services.Interfaces;

public record StatisticsResult(int Total, int Completed, int Pending, double Percentage);

public interface IStatisticsService
{
    StatisticsResult GetStatistics();
}
