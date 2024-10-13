namespace ActilityService.MQTT.Interface;

using System.Threading.Tasks;

public interface IMqttClientService
{
    Task ConnectAsync();
    Task SubscribeAsync(string topic);
    bool IsConnected();
}