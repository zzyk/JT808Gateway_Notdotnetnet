using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 应答数据消费接口(将接收到的应答数据下发给设备)
    /// </summary>
    public interface IJT808MsgReplyConsumer : IJT808PubSub, IDisposable
    {
        void OnMessage(Action<(string TerminalNo, byte[] Data)> callback);
        CancellationTokenSource Cts { get; }
        /// <summary>
        /// 订阅
        /// </summary>
        void Subscribe();
        /// <summary>
        /// 取消订阅
        /// </summary>
        void Unsubscribe();
    }
}
