using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.Kafka
{
    /// <summary>
    /// JT808消息生产配置
    /// </summary>
    public class JT808MsgProducerConfig : JT808ProducerConfig, IOptions<JT808MsgProducerConfig>
    {
        JT808MsgProducerConfig IOptions<JT808MsgProducerConfig>.Value => this;
    }
}
