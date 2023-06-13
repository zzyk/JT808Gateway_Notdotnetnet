using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Enums;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JT808.Gateway.Session
{
    /// <summary>
    /// JT808 TCP会话
    /// </summary>
    public class JT808TcpSession: IJT808Session
    {
        public JT808TcpSession(Socket client)
        {
            Client = client;
            RemoteEndPoint = client.RemoteEndPoint;
            ActiveTime = DateTime.Now;
            StartTime = DateTime.Now;
            SessionID = Guid.NewGuid().ToString("N");
            ReceiveTimeout = new CancellationTokenSource();
        }

        /// <summary>
        /// 终端手机号
        /// </summary>
        public string TerminalPhoneNo { get; set; }
        /// <summary>
        /// 活动时间
        /// </summary>
        public DateTime ActiveTime { get; set; }
        /// <summary>
        /// 起始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 传输协议
        /// </summary>
        public JT808TransportProtocolType TransportProtocolType { get;} = JT808TransportProtocolType.tcp;
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID { get; }
        /// <summary>
        /// Socket
        /// </summary>
        public Socket Client { get; set; }
        /// <summary>
        /// 接收超时
        /// </summary>
        public CancellationTokenSource ReceiveTimeout { get; set; }
        /// <summary>
        /// 远程主机
        /// </summary>
        public EndPoint RemoteEndPoint { get ; set; }

        public void Close()
        {
            try
            {
                Client.Shutdown(SocketShutdown.Both);
            }
            catch { }
            finally
            {
                Client.Close();
            }
        }
    }
}
