using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace JT808Server.Kafka
{
    [DependsOn(typeof(Volo.Abp.Kafka.AbpKafkaModule))]
    public class KafkaAppModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var kafkaConfig = context.Services.GetConfiguration().GetSection("kafka");
            Configure<KafkaConsumerConfig>(kafkaConfig.GetSection("Consumer"));
            Configure<KafkaProducerConfig>(kafkaConfig.GetSection("Producer"));
            //配置ByteValue类型
            Configure<Volo.Abp.Kafka.AbpKafkaOptions>(options =>
            {
                options.ConfigureConsumer = config =>
                {
                    config.StatisticsIntervalMs = 5000;
                    config.AutoOffsetReset = AutoOffsetReset.Earliest;
                    config.EnablePartitionEof = true;//每当消费者到达分区的末端
                };
                options.ConfigureProducer = config =>
                {
                    config.Acks = Acks.All;
                    config.EnableIdempotence = true;
                    config.LingerMs = 100;
                };
            });
        }
    }
}
