using ActilityService.KAFKA.Configuration;
using ActilityService.KAFKA.Interface;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActilityService.KAFKA;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> kafkaLogger;
    private readonly KafkaSettings kafkaSettings;
    private readonly IProducer<Null, string> producer;

    public KafkaProducerService(ILogger<KafkaProducerService> kafkaLogger, IOptions<KafkaSettings> kafkaSettings)
    {
        this.kafkaLogger = kafkaLogger;
        this.kafkaSettings = kafkaSettings.Value;

        var config = new ProducerConfig
        {
            BootstrapServers = this.kafkaSettings.BootstrapServers
        };

        this.producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        try
        {
            await CreateTopicIfNotExistsAsync(topic);

            var result = await producer.ProduceAsync(topic, new Message<Null, string> { Value = message });
            kafkaLogger.LogInformation($"Delivered '{result.Value}' to '{result.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> ex)
        {
            kafkaLogger.LogError($"Delivery failed: {ex.Error.Reason}");
        }
    }

    private async Task CreateTopicIfNotExistsAsync(string topic)
    {
        using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = kafkaSettings.BootstrapServers }).Build())
        {
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            if (metadata.Topics.Exists(t => t.Topic == topic))
            {
                kafkaLogger.LogInformation($"Topic {topic} already exists.");
                return;
            }

            await adminClient.CreateTopicsAsync(new TopicSpecification[]
            {
                        new TopicSpecification { Name = topic, NumPartitions = kafkaSettings.TopicConfigurations.NumOfPartitions, ReplicationFactor = (short)kafkaSettings.TopicConfigurations.ReplicationFactor }
            });
            kafkaLogger.LogInformation($"Topic {topic} created.");
        }
    }
}
