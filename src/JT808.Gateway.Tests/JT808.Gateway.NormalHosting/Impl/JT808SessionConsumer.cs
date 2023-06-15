﻿using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JT808.Gateway.NormalHosting.Services;

namespace JT808.Gateway.NormalHosting.Impl
{
    /// <summary>
    /// JT808会话消费者
    /// </summary>
    public class JT808SessionConsumer : IJT808SessionConsumer
    {
        public CancellationTokenSource Cts => new CancellationTokenSource();

        private readonly ILogger logger;

        public string TopicName { get; } = JT808GatewayConstants.SessionTopic;

        private readonly JT808SessionService JT808SessionService;
        public JT808SessionConsumer(
            JT808SessionService jT808SessionService,
            ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<JT808SessionConsumer>();
            JT808SessionService = jT808SessionService;
        }
        /// <summary>
        /// 接收消息处理
        /// </summary>
        /// <param name="callback"></param>
        public void OnMessage(Action<(string Notice, string TerminalNo)> callback)
        {
            new Thread((async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try
                    {
                        var item = await JT808SessionService.ReadAsync(Cts.Token);
                        callback(item);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "");
                    }
                }
            })).Start();
        }

        public void Unsubscribe()
        {
            Cts.Cancel();
        }

        public void Dispose()
        {
            Cts.Dispose();
        }

        public void Subscribe()
        {

        }
    }
}
