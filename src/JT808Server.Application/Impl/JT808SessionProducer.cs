using JT808.Gateway.Abstractions;
using JT808Server.Application.Services;

namespace JT808Server.Application.Impl
{
    /// <summary>
    /// JT808会话生产者
    /// </summary>
    public class JT808SessionProducer : IJT808SessionProducer
    {
        public string TopicName { get; } = JT808GatewayConstants.SessionTopic;

        private readonly JT808SessionService JT808SessionService;

        public JT808SessionProducer(JT808SessionService jT808SessionService)
        {
            JT808SessionService = jT808SessionService;
        }

        public async void ProduceAsync(string notice,string terminalNo)
        {
            await JT808SessionService.WriteAsync(notice, terminalNo);
        }

        public void Dispose()
        {
        }
    }
}
