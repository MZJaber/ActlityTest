namespace ActilityService.Modules;
public class ActilityMessage
{
    public DateTime Time { get; set; }
    public string DevEUI { get; set; }
    public Payload Payload { get; set; }
}

public class Payload
{
    public double GpsLatitude { get; set; }
    public double GpsLongitude { get; set; }
    public double HorizontalAccuracy { get; set; }
    public string MessageType { get; set; }
}
