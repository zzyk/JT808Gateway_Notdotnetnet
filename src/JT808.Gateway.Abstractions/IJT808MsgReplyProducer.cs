using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// JT808 应答数据生产接口（将生产的数据解析为对应的消息Id应答发送到队列）
    /// </summary>
    public interface IJT808MsgReplyProducer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminalNo">设备终端号</param>
        /// <param name="data">808 hex data</param>
        ValueTask ProduceAsync(string terminalNo, byte[] data);
    }
}
