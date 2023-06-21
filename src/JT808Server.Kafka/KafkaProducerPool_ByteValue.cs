using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Kafka;

namespace JT808Server.Kafka
{
    /// <summary>
    /// Kafka生产者对象池
    /// </summary>
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IProducerPool))]
    internal class KafkaProducerPool_ByteValue : ProducerPool
    {
        public KafkaProducerPool_ByteValue(IOptions<AbpKafkaOptions> options) : base(options)
        {

        }
        public override IProducer<string, byte[]> Get(string connectionName = null)
        {
            connectionName ??= KafkaConnections.DefaultConnectionName;
            return Producers.GetOrAdd(
                connectionName, connection => new Lazy<IProducer<string, byte[]>>(() =>
                {
                    var producerConfig = new ProducerConfig(Options.Connections.GetOrDefault(connection).ToDictionary(k => k.Key, v => v.Value));
                    Options.ConfigureProducer?.Invoke(producerConfig);
                    var producerBuilder = new ProducerBuilder<string, byte[]>(producerConfig);
                    producerBuilder.SetErrorHandler((producer, error) =>
                    {
                        base.Logger.LogError(error.Reason);
                    });
                    return producerBuilder.Build();
                })).Value;
        }
    }
}
