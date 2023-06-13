using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 数据消费接口
    /// </summary>
    public interface IJT808MsgConsumer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 
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
