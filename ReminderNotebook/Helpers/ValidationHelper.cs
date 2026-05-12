namespace ReminderNotebook.Helpers;

public static class ValidationHelper
{
    public static void RequireNonEmpty(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    }

    public static void RequirePositive(int value, string paramName)
    {
        if (value <= 0)
            throw new ArgumentException($"{paramName} must be positive.", paramName);
    }

    public static void RequireFutureDate(DateTime value, string paramName)
    {
        if (value <= DateTime.Now)
            throw new ArgumentException($"{paramName} must be in the future.", paramName);
    }

    public static bool IsValidColorHex(string colorHex) =>
        !string.IsNullOrEmpty(colorHex) &&
        colorHex.StartsWith('#') &&
        colorHex.Length == 7;
}