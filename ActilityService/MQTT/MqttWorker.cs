namespace ActilityService.MQTT;

using ActilityService.MQTT.Configurations;
using ActilityService.MQTT.Interface;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

public class MqttWorker : BackgroundService
{
    private readonly ILogger<MqttWorker> mqttWorkerLogger;
    private readonly IMqttClientService mqttClientService;
    private readonly MqttSettings mqttSettings;


    public MqttWorker(ILogger<MqttWorker> mqttWorkerLogger, IMqttClientService mqttClientService, IOptions<MqttSettings> mqttSettings)
    {
        this.mqttWorkerLogger = mqttWorkerLogger;
        this.mqttClientService = mqttClientService;
        this.mqttSettings = mqttSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        mqttWorkerLogger.LogInformation("MqttWorker running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!mqttClientService.IsConnected())
            {
                await mqttClientService.ConnectAsync();
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        mqttWorkerLogger.LogInformation("MqttWorker stopping.");
        await base.StopAsync(stoppingToken);
    }
}
