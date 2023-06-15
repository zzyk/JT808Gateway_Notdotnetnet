using System;
using System.Threading.Tasks;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 会话通知（在线/离线）生产接口
    /// </summary>
    public interface IJT808SessionProducer : IJT808PubSub, IDisposable
    {
        /// <summary>
        /// 异步生产消息
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="terminalNo"></param>
        void ProduceAsync(string notice,string terminalNo);
    }
}
