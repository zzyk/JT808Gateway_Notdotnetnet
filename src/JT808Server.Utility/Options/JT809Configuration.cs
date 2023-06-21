using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility.Options
{
    public class JT809Configuration
    {
        /// <summary>
        /// 服务ip
        /// </summary>
        public string Server { get; set; } = "127.0.0.1";
        public int TcpPort_2011 { get; set; } = 809;
        public int TcpPort_2019 { get; set; } = 810;
        /// <summary>
        /// 是否跳过验证校验码
        /// </summary>
        public bool SkipCRCCode { get; set; }
        /// <summary>
        /// 默认开启从链路连接客户端
        /// </summary>
        public bool SubordinateClientEnable { get; set; } = true;
        /// <summary>
        /// 0:国标 1:苏标 2:新苏标 3:四川标
        /// </summary>
        public int AlarmAnalysis { get; set; } = 0;
        /// <summary>
        /// 是否启用行为分析附件自动下载功能
        /// </summary>
        public bool IsDownFTP { get; set; } = false;

        /// <summary>
        /// 附件下载次数
        /// </summary>
        public int DownCount { get; set; } = 1;

        /// <summary>
        /// 0x9404从链路发送时间间隔,单位秒
        /// </summary>
        public int IntervalTime { get; set; } = 20;

        /// <summary>
        /// 是否下发车辆报文请求 0:不发 1:下发
        /// </summary>
        public int DownMsgVehicle { get; set; } = 0;


        public int QuietPeriodSeconds { get; set; } = 1;
        public TimeSpan QuietPeriodTimeSpan => TimeSpan.FromSeconds(QuietPeriodSeconds);
        public int ShutdownTimeoutSeconds { get; set; } = 3;
        public TimeSpan ShutdownTimeoutTimeSpan => TimeSpan.FromSeconds(ShutdownTimeoutSeconds);
        public int SoBacklog { get; set; } = 8192;
        public int EventLoopCount { get; set; } = Environment.ProcessorCount;
        public int ReaderIdleTimeSeconds { get; set; } = 3600;
        public int WriterIdleTimeSeconds { get; set; } = 3600;
        public int AllIdleTimeSeconds { get; set; } = 3600;
        /// <summary>
        /// 转发远程地址 (可选项)知道转发的地址有利于提升性能
        /// 按照808的消息，有些请求必须要应答，但是转发可以不需要有应答可以节省部分资源包括：
        //  1.消息的序列化
        //  2.消息的下发
        //  都有一定的性能损耗，那么不需要判断写超时 IdleState.WriterIdle
        //  就跟神兽貔貅一样。。。
        /// </summary>
        public List<string> ForwardingRemoteIPAddress { get; set; }

        /// <summary>
        /// 是否开启车辆和平台对应绑定
        /// </summary>
        public bool IsPlatformVehicleBind { get; set; }
    }
}
