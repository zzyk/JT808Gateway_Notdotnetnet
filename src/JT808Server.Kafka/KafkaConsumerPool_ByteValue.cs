using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Kafka;

namespace JT808Server.Kafka
{
    /// <summary>
    /// Kafka消费者对象池
    /// </summary>
    [Dependency(ReplaceServices = true)]
    [ExposeServices(typeof(IConsumerPool))]
    internal class KafkaConsumerPool_ByteValue : ConsumerPool
    {
        public KafkaConsumerPool_ByteValue(IOptions<AbpKafkaOptions> options) : base(options)
        {

        }
        public override IConsumer<string, byte[]> Get(string groupId, string connectionName = null)
        {
            connectionName ??= KafkaConnections.DefaultConnectionName;
            var keyStr = $"{connectionName}_{groupId}";
            return Consumers.GetOrAdd(
                keyStr, key => new Lazy<IConsumer<string, byte[]>>(() =>
                {
                    //引发了线程安全问题 https://github.com/abpframework/abp/issues/13897
                    //官方解决方式 .ToDictionary(k => k.Key, v => v.Value)
                    var connection = key.Split('_')[0];
                    var config = new ConsumerConfig(Options.Connections.GetOrDefault(connection).ToDictionary(k => k.Key, v => v.Value))
                    {
                        GroupId = groupId,
                        EnableAutoCommit = false,
                        AutoOffsetReset = AutoOffsetReset.Earliest,
                        //EnableAutoOffsetStore = false//<----this
                        //PartitionAssignmentStrategy = PartitionAssignmentStrategy.CooperativeSticky //消费者粘性策略
                    };
                    Options.ConfigureConsumer?.Invoke(config);
                    var consumerBuilder = new ConsumerBuilder<string, byte[]>(config);
                    consumerBuilder.SetErrorHandler((consumer, error) =>
                    {
                        base.Logger.LogError(error.Reason);
                    });
                    return consumerBuilder.Build();
                })
            ).Value;
        }
    }
}
