﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 会话通知（在线/离线）
    /// </summary>
    public interface IJT808SessionConsumer : IJT808PubSub, IDisposable
    {
        void OnMessage(Action<(string Notice, string TerminalNo)> callback);
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
