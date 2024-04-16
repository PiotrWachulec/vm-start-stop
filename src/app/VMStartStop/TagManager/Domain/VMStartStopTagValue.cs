namespace MyCo.TagManager.Domain;

public class VMStartStopTagValue
{
    public bool IsOn { get; }
    public string StartTime { get; }
    public string EndTime { get; }
    public string TimeZone { get; }
    public string Days { get; }

    public VMStartStopTagValue(string tagValue)
    {
        string[] tagValues = tagValue.Split(";");
        IsOn = tagValues[0].Equals("ON");
        TimeZone = tagValues[2];
        Days = tagValues[3];

        var timeValues = tagValues[1].Split("-");
        StartTime = timeValues[0];
        EndTime = timeValues[1];
    }
}