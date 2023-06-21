namespace JT808Server.Kafka
{
    /// <summary>
    ///  Kafka 消费配置
    /// </summary>
    public class KafkaConsumerConfig
    {
        public TopicConsumer _DownMsgToVehicleTopic { get; set; }

    }
    public class TopicConsumer
    {
        /// <summary>
        /// 主题
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 消费者分组
        /// </summary>
        public string GroupId { get; set; }
    }
}
