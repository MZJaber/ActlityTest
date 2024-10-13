namespace ActilityService.KAFKA.Configuration;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public TopicConfigurations TopicConfigurations { get; set; }
}

public class TopicConfigurations
{
    public string Topic { get; set; }
    public int ReplicationFactor { get; set; }
    public int NumOfPartitions { get; set; }
}