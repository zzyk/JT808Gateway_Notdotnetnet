using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.Kafka
{
    /// <summary>
    /// JT808消息消费配置
    /// </summary>
    public class JT808MsgConsumerConfig : JT808ConsumerConfig, IOptions<JT808MsgConsumerConfig>
    {
        JT808MsgConsumerConfig IOptions<JT808MsgConsumerConfig>.Value => this;
    }
}
