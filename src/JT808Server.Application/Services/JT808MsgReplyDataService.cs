using System.Threading.Channels;
using Volo.Abp.DependencyInjection;

namespace JT808Server.Application.Services
{
    /// <summary>
    /// JT808上下行消息应答数据服务
    /// </summary>
    public class JT808MsgReplyDataService : ISingletonDependency
    {
        private readonly Channel<(string TerminalNo, byte[] Data)> _channel;

        public JT808MsgReplyDataService()
        {
            _channel = Channel.CreateUnbounded<(string TerminalNo, byte[] Data)>();
        }

        public async ValueTask WriteAsync(string terminalNo, byte[] Data)
        {
            await _channel.Writer.WriteAsync((terminalNo, Data));
        }
        public async ValueTask<(string TerminalNo, byte[] Data)> ReadAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
