using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.Kafka
{
    /// <summary>
    /// JT808 消息应答日志消费配置
    /// </summary>
    public class JT808MsgReplyLoggingConsumerConfig : JT808ConsumerConfig, IOptions<JT808MsgReplyLoggingConsumerConfig>
    {
        JT808MsgReplyLoggingConsumerConfig IOptions<JT808MsgReplyLoggingConsumerConfig>.Value => this;
    }
}
