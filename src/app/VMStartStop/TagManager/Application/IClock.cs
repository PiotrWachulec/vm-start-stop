namespace MyCo.TagManager.Application;

public interface IClock
{
    public TimeOnly ConvertUtcToLocalTime(TimeOnly time, string timeZone);
    public IList<TimeZoneInfo> GetTimeZones();
}