namespace ActilityService.KAFKA;

using ActilityService.Cash;
using ActilityService.KAFKA.Configuration;
using ActilityService.KAFKA.Interface;
using ActilityService.Modules;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

public class KafkaProducerWorker : BackgroundService
{
    private readonly ILogger<KafkaProducerWorker> kafkaWorkerLogger;
    private readonly IKafkaProducerService kafkaProducerService;
    private readonly KafkaSettings kafkaSettings;

    public KafkaProducerWorker(ILogger<KafkaProducerWorker> kafkaWorkerLogger, IKafkaProducerService kafkaProducerService, IOptions<KafkaSettings> kafkaSettings)
    {
        this.kafkaWorkerLogger = kafkaWorkerLogger;
        this.kafkaProducerService = kafkaProducerService;
        this.kafkaSettings = kafkaSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        kafkaWorkerLogger.LogInformation("KafkaProducerWorker running at: {time}", DateTimeOffset.Now);

        while (!stoppingToken.IsCancellationRequested)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var messages = new List<PEPayloadMessage>();

                while (MessageQueueCash.MessageQueue.TryDequeue(out var pePayloadMessage))
                {
                    messages.Add(pePayloadMessage);
                }

                if (messages.Any())
                {
                    var messageJson = JsonSerializer.Serialize(messages);
                    await kafkaProducerService.ProduceAsync(kafkaSettings.TopicConfigurations.Topic, messageJson);
                    kafkaWorkerLogger.LogInformation($"Produced message to Kafka: {messageJson}");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        kafkaWorkerLogger.LogInformation("KafkaProducerWorker stopping.");
        await base.StopAsync(stoppingToken);
    }
}
