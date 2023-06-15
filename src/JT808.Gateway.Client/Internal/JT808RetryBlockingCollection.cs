using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Client.Internal
{
    /// <summary>
    /// JT808 重试阻塞集合
    /// </summary>
    public class JT808RetryBlockingCollection
    {
        public BlockingCollection<JT808DeviceConfig> RetryBlockingCollection { get; }

        public JT808RetryBlockingCollection()
        {
            RetryBlockingCollection = new BlockingCollection<JT808DeviceConfig>(999999);
        }
    }
}
