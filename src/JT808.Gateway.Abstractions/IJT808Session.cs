using JT808.Gateway.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// JT808会话
    /// </summary>
    public interface IJT808Session
    {
        /// <summary>
        /// 终端手机号
        /// </summary>
        string TerminalPhoneNo { get; set; }
        /// <summary>
        /// 会话id
        /// </summary>
        string SessionID { get; }
        /// <summary>
        /// socket
        /// </summary>
        Socket Client { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime StartTime { get; set; }
        /// <summary>
        /// 活动时间
        /// </summary>
        DateTime ActiveTime { get; set; }
        /// <summary>
        /// 传输协议类型
        /// </summary>
        JT808TransportProtocolType TransportProtocolType { get;}
        /// <summary>
        /// 接收超时
        /// </summary>
        CancellationTokenSource ReceiveTimeout { get; set; }
        /// <summary>
        /// 远程终结点
        /// </summary>
        EndPoint RemoteEndPoint { get; set; }
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}
