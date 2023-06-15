﻿using JT808.Gateway.Client.Metadata;

namespace JT808.Gateway.Client.Services
{
    /// <summary>
    /// 发送计数包服务
    /// </summary>
    public class JT808SendAtomicCounterService
    {
        /// <summary>
        /// 成功消息数
        /// </summary>
        private readonly JT808AtomicCounter MsgSuccessCounter;

        public JT808SendAtomicCounterService()
        {
            MsgSuccessCounter=new JT808AtomicCounter();
        }

        public void Reset()
        {
            MsgSuccessCounter.Reset();
        }

        public long MsgSuccessIncrement()
        {
            return MsgSuccessCounter.Increment();
        }

        public long MsgSuccessCount
        {
            get
            {
                return MsgSuccessCounter.Count;
            }
        }
    }
}
