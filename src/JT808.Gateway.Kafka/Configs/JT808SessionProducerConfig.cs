using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.Kafka
{
    /// <summary>
    /// JT808会话生产配置
    /// </summary>
    public class JT808SessionProducerConfig : JT808ProducerConfig, IOptions<JT808SessionProducerConfig>
    {
        JT808SessionProducerConfig IOptions<JT808SessionProducerConfig>.Value => this;
    }
}
