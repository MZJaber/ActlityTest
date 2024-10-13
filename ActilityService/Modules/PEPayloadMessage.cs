namespace ActilityService.Modules;

public class PEPayloadMessage
{
    public string source { get; set; }
    public string type { get; set; }
    public string transactionTime { get; set; }

    public ActilityMessage Data { get; set; }

    public PEPayloadMessage(ActilityMessage data)
    {
        this.source = "Actlity";
        this.type = "Position";
        this.transactionTime = data.Time.ToString();
        Data = data;
    }
}
