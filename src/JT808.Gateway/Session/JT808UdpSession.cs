﻿using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Enums;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JT808.Gateway.Session
{
    /// <summary>
    /// JT808 UDP 会话
    /// </summary>
    public class JT808UdpSession: IJT808Session
    {
        public JT808UdpSession(Socket socket, EndPoint sender)
        {
            ActiveTime = DateTime.Now;
            StartTime = DateTime.Now;
            SessionID = Guid.NewGuid().ToString("N");
            ReceiveTimeout = new CancellationTokenSource();
            RemoteEndPoint = sender;
            Client = socket;
        }

        /// <summary>
        /// 终端手机号
        /// </summary>
        public string TerminalPhoneNo { get; set; }
        public DateTime ActiveTime { get; set; }
        public DateTime StartTime { get; set; }
        public JT808TransportProtocolType TransportProtocolType { get; set; } = JT808TransportProtocolType.udp;

        public string SessionID { get; }

        public Socket Client { get; set; }
        public CancellationTokenSource ReceiveTimeout { get; set; }
        public EndPoint RemoteEndPoint { get; set ; }

        public void Close()
        {
            
        }
    }
}
