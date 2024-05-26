using MyCo.TagManager.Application;

namespace MyCo.TagManager.Infrastrucutre;

public class Clock : IClock
{
    public TimeOnly ConvertUtcToLocalTime(TimeOnly time, string timeZone)
    {
        var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
        var utcTime = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, time.Hour, time.Minute, time.Second);
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZoneInfo);
        return TimeOnly.FromDateTime(localTime);
    }
}