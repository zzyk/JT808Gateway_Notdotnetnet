using JT808.Protocol;
using JT808.Protocol.Enums;
using JT808.Protocol.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Client
{
    /// <summary>
    /// JT808 设备配置
    /// </summary>
    public class JT808DeviceConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminalPhoneNo">SIM卡号</param>
        /// <param name="tcpHost">TCP IP</param>
        /// <param name="tcpPort">TCP 端口号</param>
        /// <param name="localIPAddress">本机IP</param>
        /// <param name="localPort">本机端口号</param>
        /// <param name="version">版本</param>
        public JT808DeviceConfig(string terminalPhoneNo, string tcpHost,int tcpPort, string localIPAddress=null,int localPort=0, JT808Version version= JT808Version.JTT2013)
        {
            TerminalPhoneNo = terminalPhoneNo;
            TcpHost = tcpHost;
            TcpPort = tcpPort;
            Version = version;
            LocalIPAddress = localIPAddress;
            LocalPort = localPort;
        }
        public JT808Version Version { get; private set; }
        public string TerminalPhoneNo { get; private set; }
        public string TcpHost { get; private set; }
        public int TcpPort { get; private set; }
        /// <summary>
        /// 心跳时间（秒）
        /// </summary>
        public int Heartbeat { get; set; } = 30;
        /// <summary>
        /// 自动重连 默认true
        /// </summary>
        public bool AutoReconnection { get; set; } = true;
        /// <summary>
        /// JT808分布式自增流水号
        /// </summary>
        public IJT808MsgSNDistributed MsgSNDistributed { get; }
        public string LocalIPAddress { get; set; }
        public int LocalPort { get; set; }
    }
}
