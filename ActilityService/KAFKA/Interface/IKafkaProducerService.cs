namespace ActilityService.KAFKA.Interface;

public interface IKafkaProducerService
{
    Task ProduceAsync(string topic, string message);
}
