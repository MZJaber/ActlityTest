namespace MqttWorkerService;

using ActilityService.KAFKA.Configuration;
using ActilityService.KAFKA.Interface;
using ActilityService.KAFKA;
using ActilityService.MQTT;
using ActilityService.MQTT.Configurations;
using ActilityService.MQTT.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<MqttSettings>(hostContext.Configuration.GetSection("MqttSettings"));
                services.Configure<KafkaSettings>(hostContext.Configuration.GetSection("Kafka"));

                services.AddSingleton<IMqttClientService, MqttClientService>();
                services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
                services.AddHostedService<MqttWorker>();
                services.AddHostedService<KafkaProducerWorker>();
            })
            .Build();

        await host.RunAsync();
    }
}
