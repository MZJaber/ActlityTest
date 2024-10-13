using ActilityService.MQTT.Configurations;
using ActilityService.MQTT.Interface;
using Microsoft.Extensions.Logging;
using MQTTnet.Client;
using MQTTnet;
using Microsoft.Extensions.Options;
using ActilityService.Cash;
using ActilityService.Modules;
using System.Text.Json;
using ActilityService.Shared;

namespace ActilityService.MQTT;

public class MqttClientService : IMqttClientService
{
    private readonly IMqttClient mqttClient;
    private readonly ILogger<MqttClientService> mqttLogger;
    private readonly MqttSettings mqttSettings;

    public MqttClientService(ILogger<MqttClientService> mqttLogger, IOptions<MqttSettings> mqttSettings)
    {
        this.mqttLogger = mqttLogger;
        this.mqttSettings = mqttSettings.Value;
        var factory = new MqttFactory();
        this.mqttClient = factory.CreateMqttClient();
    }

    public async Task ConnectAsync()
    {
        var options = new MqttClientOptionsBuilder()
            .WithClientId(mqttSettings.ClientId)
            .WithTcpServer(mqttSettings.BrokerHost, mqttSettings.BrokerPort)
            .WithCredentials(mqttSettings.Username, mqttSettings.Password)
            .Build();

        await mqttClient.ConnectAsync(options);

        await SubscribeAsync(mqttSettings.Topic);

        mqttClient.ApplicationMessageReceivedAsync += e =>
        {
            var message = e.ApplicationMessage;

            if (message is not null)
            {
                var payloadAsString = System.Text.Encoding.UTF8.GetString(message.Payload);

                mqttLogger.LogInformation($"Received message: {payloadAsString} from topic: {message.Topic}");

                try
                {
                    var actilityMessage = JsonSerializer.Deserialize<ActilityMessage>(payloadAsString);

                    if (actilityMessage != null && actilityMessage.Payload.MessageType == SheardConstants.MessageType)
                    {
                        var peMessage = new PEPayloadMessage(actilityMessage);
                        MessageQueueCash.MessageQueue.Enqueue(peMessage);
                    }
                }
                catch (JsonException ex)
                {
                    mqttLogger.LogError(ex, "Failed to deserialize message.");
                }
            }

            return Task.CompletedTask;
        };
    }

    public async Task SubscribeAsync(string topic)
    {
        if (!mqttClient.IsConnected) return;

        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(topic)
            .WithAtMostOnceQoS()
            .Build());
    }

    public bool IsConnected() => mqttClient.IsConnected;
}
