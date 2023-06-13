using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Services;

namespace JT808.Gateway.HostedServices
{
    /// <summary>
    /// 会话通知（在线/离线）托管服务
    /// </summary>
    public class JT808SessionNoticeHostedService : IHostedService
    {
        private readonly JT808SessionNoticeService jT808SessionNoticeService;
        private readonly IJT808SessionConsumer jT808SessionConsumer;
        public JT808SessionNoticeHostedService(
            IJT808SessionConsumer jT808SessionConsumer,
            JT808SessionNoticeService jT808SessionNoticeService)
        {
            this.jT808SessionNoticeService = jT808SessionNoticeService;
            this.jT808SessionConsumer = jT808SessionConsumer;
        }
        /// <summary>
        /// 异步开始
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            jT808SessionConsumer.Subscribe();
            jT808SessionConsumer.OnMessage(jT808SessionNoticeService.Processor);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 异步停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            jT808SessionConsumer.Unsubscribe();
            return Task.CompletedTask;
        }
    }
}
