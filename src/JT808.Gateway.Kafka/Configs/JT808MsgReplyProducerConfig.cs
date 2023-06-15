using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Configs.Kafka
{
    /// <summary>
    /// JT808 消息应答生产配置
    /// </summary>
    public class JT808MsgReplyProducerConfig : JT808ProducerConfig, IOptions<JT808MsgReplyProducerConfig>
    {
        JT808MsgReplyProducerConfig IOptions<JT808MsgReplyProducerConfig>.Value => this;
    }
}
