using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 网关应答数据日志生产接口(将网关能解析到直接能下发的数据发送到队列)
    /// </summary>
    public interface IJT808MsgReplyLoggingProducer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="terminalNo">设备终端号</param>
        /// <param name="data">808 hex data</param>
        void ProduceAsync(string terminalNo, byte[] data);
    }
}
