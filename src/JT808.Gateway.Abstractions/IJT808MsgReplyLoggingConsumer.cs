using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 网关应答数据日志消费接口（将网关能解析到直接能下发的数据发送到日志系统）
    /// </summary>
    public interface IJT808MsgReplyLoggingConsumer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 消息事件
        /// </summary>
        /// <param name="callback"></param>
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
