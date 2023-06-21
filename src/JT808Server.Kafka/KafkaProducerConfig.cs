namespace JT808Server.Kafka
{
    /// <summary>
    ///  Kafka 生产配置
    /// </summary>
    public class KafkaProducerConfig
    {
        /// <summary>
        /// GPS给报警程序使用的队列
        /// </summary>
        public string _GPSTopic { get; set; }
        /// <summary>
        /// 终端报警给报警程序使用的队列
        /// </summary>
        public string _TerminalFatigueTopic { get; set; }
        public string _KafkaDriverICTopic { get; set; }
        public string _809Server_ToKafka_Cmd1402Topic { get; set; }
        public string _809Server_ToKafka_Cmd1403Topic { get; set; }
        public string _809Server_ToKafka_Cmd1404Topic { get; set; }
        public string _Cmd1801Topic { get; set; }
        public string _809Server_ToKafka_OnlineOperationListTopic { get; set; }
        public string _809Server_ToKafka_OperationStateChangeTopic { get; set; }

        /// <summary>
        /// 808协议200指令主题
        /// </summary>
        public string _JT808_0x200Topic { get; set; }
        /// <summary>
        /// 808协议终端报警指令主题
        /// </summary>
        public string _JT808NewAlarmTopic { get; set; }
        /// <summary>
        /// 808协议主动安全报警ADAS指令主题
        /// </summary>
        public string _JT808ADASTopic { get; set; }
        /// <summary>
        /// 808协议主动安全报警DSM指令主题
        /// </summary>
        public string _JT808DSMTopic { get; set; }
    }
}
