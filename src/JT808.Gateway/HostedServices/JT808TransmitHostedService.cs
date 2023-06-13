using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;
using JT808.Gateway.Abstractions;
using JT808.Gateway.Services;

namespace JT808.Gateway.HostedServices
{
    /// <summary>
    /// 转发托管服务
    /// </summary>
    public class JT808TransmitHostedService:IHostedService
    {
        /// <summary>
        /// 转发服务
        /// </summary>
        private readonly JT808TransmitService jT808TransmitService;
        /// <summary>
        /// 数据消费接口
        /// </summary>
        private readonly IJT808MsgConsumer jT808MsgConsumer;
        public JT808TransmitHostedService(
            IJT808MsgConsumer jT808MsgConsumer,
            JT808TransmitService jT808TransmitService)
        {
            this.jT808TransmitService = jT808TransmitService;
            this.jT808MsgConsumer = jT808MsgConsumer;
        }
        /// <summary>
        /// 异步开始
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            jT808MsgConsumer.Subscribe();
            jT808MsgConsumer.OnMessage(jT808TransmitService.SendAsync);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 异步停止
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            jT808MsgConsumer.Unsubscribe();
            jT808TransmitService.Stop();
            return Task.CompletedTask;
        }
    }
}
