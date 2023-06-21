using JT808Server.Kafka;
using Volo.Abp.Modularity;

namespace JT808Server.Domain
{
    [DependsOn(typeof(KafkaAppModule))]
    public class JT808ServerDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {

        }
    }
}