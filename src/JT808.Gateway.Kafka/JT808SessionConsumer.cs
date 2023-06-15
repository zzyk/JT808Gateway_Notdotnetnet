﻿using Confluent.Kafka;
using JT808.Gateway.Configs.Kafka;
using JT808.Gateway.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JT808.Gateway.Kafka
{
    /// <summary>
    /// JT808会话消费者
    /// </summary>
    public sealed class JT808SessionConsumer : IJT808SessionConsumer
    {
        private bool disposed = false;
        public CancellationTokenSource Cts { get; private set; } = new CancellationTokenSource();

        private readonly IConsumer<string, string> consumer;

        private readonly ILogger logger;

        public string TopicName { get; }

        public JT808SessionConsumer(
            IOptions<JT808SessionConsumerConfig> consumerConfigAccessor,
            ILoggerFactory loggerFactory)
        {
            consumer = new ConsumerBuilder<string, string>(consumerConfigAccessor.Value).Build();
            TopicName = consumerConfigAccessor.Value.TopicName;
            logger = loggerFactory.CreateLogger<JT808SessionConsumer>();
        }
        /// <summary>
        /// 会话消费者消息处理
        /// </summary>
        /// <param name="callback"></param>
        public void OnMessage(Action<(string Notice, string TerminalNo)> callback)
        {
            new Thread(() =>
            {
                while (!Cts.IsCancellationRequested)
                {
                    if (disposed) return;
                    try
                    {
                        //如果不指定分区，根据kafka的机制会从多个分区中拉取数据
                        //如果指定分区，根据kafka的机制会从相应的分区中拉取数据
                        var data = consumer.Consume(Cts.Token);
                        if (logger.IsEnabled(LogLevel.Debug))
                        {
                            logger.LogDebug($"Topic: {data.Topic} Key: {data.Message.Key} Partition: {data.Partition} Offset: {data.Offset} TopicPartitionOffset:{data.TopicPartitionOffset}");
                        }
                        callback((data.Message.Key, data.Message.Value));
                    }
                    catch (OperationCanceledException ex)
                    {
                        logger.LogError(ex, TopicName);
                        break;
                    }
                    catch (ConsumeException ex)
                    {
                        logger.LogError(ex, TopicName);
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, TopicName);
                        break;
                    }
                }
            }).Start();
        }

        public void Subscribe()
        {
            consumer.Subscribe(TopicName);
        }

        public void Unsubscribe()
        {
            if (disposed) return;
            consumer.Unsubscribe();
            Cts.Cancel();
        }

        private void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                consumer.Close();
                consumer.Dispose();
                Cts.Dispose();
            }
            disposed = true;
        }
        ~JT808SessionConsumer()
        {
            Dispose(false);
        }
        public void Dispose()
        {
            //必须为true
            Dispose(true);
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }
    }
}
