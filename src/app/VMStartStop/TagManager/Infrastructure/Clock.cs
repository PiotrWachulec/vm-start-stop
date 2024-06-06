using MyCo.TagManager.Application;

namespace MyCo.TagManager.Infrastrucutre;

public class Clock : IClock
{
    public TimeOnly ConvertUtcToLocalTime(TimeOnly time, string timeZone)
    {
        var timeZoneInfo = GetTimeZoneInfo(timeZone);

        var utcTime = new DateTime(
            DateTime.UtcNow.Year,
            DateTime.UtcNow.Month,
            DateTime.UtcNow.Day,
            time.Hour,
            time.Minute,
            time.Second);

        var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
        return TimeOnly.FromDateTime(localTime);
    }

    public IList<TimeZoneInfo> GetTimeZones()
    {
        return TimeZoneInfo.GetSystemTimeZones().ToList();
    }

    private static TimeZoneInfo GetTimeZoneInfo(string timeZone)
    {
        return TimeZoneInfo.TryFindSystemTimeZoneById(timeZone, out TimeZoneInfo? timeZoneInfo)
            ? timeZoneInfo
            : TimeZoneInfo.TryConvertIanaIdToWindowsId(timeZone, out string? windowsTimeZoneId)
                ? TimeZoneInfo.FindSystemTimeZoneById(windowsTimeZoneId)
                : throw new ArgumentException($"Invalid timezone: {timeZone}");
    }
}