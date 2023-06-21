﻿using JT808.Gateway.Abstractions;
using JT808Server.Application.Services;
using Microsoft.Extensions.Logging;

namespace JT808Server.Application.Impl
{
    /// <summary>
    /// JT808 消息应答消费者
    /// </summary>
    public class JT808MsgReplyConsumer : IJT808MsgReplyConsumer
    {
        public CancellationTokenSource Cts { get; } = new CancellationTokenSource();

        public string TopicName { get; } = JT808GatewayConstants.MsgReplyTopic;

        private readonly JT808MsgReplyDataService MsgReplyDataService;

        private ILogger logger;

        public JT808MsgReplyConsumer(
            ILoggerFactory loggerFactory,
            JT808MsgReplyDataService msgReplyDataService)
        {
            MsgReplyDataService = msgReplyDataService;
            logger = loggerFactory.CreateLogger<JT808MsgReplyConsumer>();
        }

        public void Dispose()
        {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        public void OnMessage(Action<(string TerminalNo, byte[] Data)> callback)
        {
            new Thread(async () =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    try
                    {
                        var item = await MsgReplyDataService.ReadAsync(Cts.Token);
                        callback(item);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "");
                    }
                }
            }).Start();
        }

        public void Subscribe()
        {
            
        }

        public void Unsubscribe()
        {
            
        }
    }
}
