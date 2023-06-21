using System.Threading.Channels;
using Volo.Abp.DependencyInjection;

namespace JT808Server.Application.Services
{
    /// <summary>
    /// JT808会话服务
    /// </summary>
    public class JT808SessionService : ISingletonDependency
    {
        private readonly Channel<(string Notice, string TerminalNo)> _channel;

        public JT808SessionService()
        {
            _channel = Channel.CreateUnbounded<(string Notice, string TerminalNo)>();
        }

        public async ValueTask WriteAsync(string notice, string terminalNo)
        {
            await _channel.Writer.WriteAsync((notice, terminalNo));
        }
        public async ValueTask<(string Notice, string TerminalNo)> ReadAsync(CancellationToken cancellationToken)
        {
            return await _channel.Reader.ReadAsync(cancellationToken);
        }
    }
}
