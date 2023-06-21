namespace JT808Server.Utility.Options
{
    public class SystemConfiguration
    {
        /// <summary>
        /// 数据库连接字符串集合
        /// </summary>
        public Dictionary<string, string> ConnStrings { get; set; }

        /// <summary>
        /// 安标接口
        /// </summary>
        public string AnBiaoUrl { get; set; }
        /// <summary>
        /// 安标车辆子接口
        /// </summary>
        public string GetVehicleUrl { get; set; }

        /// <summary>
        /// Redis地址
        /// </summary>
        public string RedisConfig { get; set; }
        /// <summary>
        /// 日志保留天数
        /// </summary>
        public int LogSaveDay { get; set; }

        /// <summary>
        /// GC强制回收计时器执行间隔时间分钟
        /// </summary>
        public int GCExecutionTime { get; set; }

        /// <summary>
        /// 更新运营商传输数据记录刷新时间秒
        /// </summary>
        public int ConnStateRefreshTime { get; set; }
        
        /// <summary>
        /// GPS分区个数
        /// </summary>
        public int GPSPartitionCount { get; set; }
        /// <summary>
        /// 主动安全报警分区个数
        /// </summary>
        public int ADAS_DSM_PartitionCount { get; set; }
    }
}
