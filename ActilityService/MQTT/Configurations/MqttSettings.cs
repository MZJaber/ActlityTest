namespace ActilityService.MQTT.Configurations;
public class MqttSettings
{
    public string BrokerHost { get; set; }
    public int BrokerPort { get; set; }
    public string ClientId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Topic { get; set; }
}
