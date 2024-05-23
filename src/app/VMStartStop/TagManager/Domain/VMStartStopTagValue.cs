namespace MyCo.TagManager.Domain;

public class VMStartStopTagValue
{
    public bool IsOn { get; }
    public TimeOnly StartTime { get; }
    public TimeOnly EndTime { get; }
    public string TimeZone { get; }
    public string Days { get; }

    public VMStartStopTagValue(string tagValue)
    {
        string[] tagValues = tagValue.Split(";");
        IsOn = tagValues[0].Equals("ON");
        TimeZone = tagValues[2];
        Days = tagValues[3];

        var timeValues = tagValues[1].Split("-");
        StartTime = TimeOnly.FromTimeSpan(TimeSpan.Parse(timeValues[0]));
        EndTime = TimeOnly.FromTimeSpan(TimeSpan.Parse(timeValues[1]));
    }
}