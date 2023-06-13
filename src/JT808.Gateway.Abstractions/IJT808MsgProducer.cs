using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// JT808 数据生产接口（网关将接收到的数据发送到队列）
    /// </summary>
    public interface IJT808MsgProducer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminalNo">设备终端号</param>
        /// <param name="data">808 hex data</param>
        void ProduceAsync(string terminalNo, byte[] data);
    }
}
