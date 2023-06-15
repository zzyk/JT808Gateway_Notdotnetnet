using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Client.Metadata
{
    /// <summary>
    /// JT808报表
    /// </summary>
    public class JT808Report
    {
        /// <summary>
        /// 发送总数
        /// </summary>
        public long SendTotalCount { get; set; }
        /// <summary>
        /// 接收总数
        /// </summary>
        public long ReceiveTotalCount { get; set; }
        /// <summary>
        /// 当前日期
        /// </summary>
        public DateTime CurrentDate { get; set; }
        /// <summary>
        /// 连接数
        /// </summary>
        public int Connections { get; set; }
        /// <summary>
        /// 在线连接数
        /// </summary>
        public int OnlineConnections { get; set; }
        /// <summary>
        /// 离线连接数
        /// </summary>
        public int OfflineConnections { get; set; }
    }
}
