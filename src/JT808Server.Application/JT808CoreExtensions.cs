using JT808.Gateway;
using JT808.Gateway.Abstractions.Configurations;
using JT808.Gateway.Session;
using JT808.Protocol;
using JT808Server.Application.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using JT808.Gateway.Services;
using JT808Server.Application.Services;
using JT808.Gateway.Client.Services;

namespace JT808Server.Application
{
    public static class JT808CoreExtensions
    {
        public static IJT808Builder AddJT808Core(this IJT808Builder builder, IConfiguration configuration = null)
        {
            if (configuration != null)
            {
                builder.Services.Configure<JT808Configuration>(configuration.GetSection("JT808Configuration"));
            }
            //客户端测试  依赖AddClient()服务
            builder.Services.AddSingleton<UpJob>();
            builder.Services.AddSingleton<CallHttpClientJob>();
            //JT808 TCP和UDP服务器
            builder.Services.AddSingleton<TCPServer>();
            builder.Services.AddSingleton<JT808UdpServer>();
            builder.Services.AddSingleton<JT808ReceiveAtomicCounterService>();
            //builder.Services.AddSingleton<JT808TcpReceiveTimeoutHostedService>();

            ////JT809计数器服务工厂
            //builder.Services.TryAddSingleton<JT808AtomicCounterServiceFactory>();
            ////JT809解码器
            //builder.Services.TryAddScoped<JT808Decoder>();
            return builder;
        }

        /// <summary>
        /// 上级平台
        /// 主链路为服务端
        /// 从链路为客户端
        /// </summary>
        /// <param name="serviceDescriptors"></param>
        /// <returns></returns>
        public static IJT808Builder AddJT808SuperiorPlatform(this IJT808Builder builder, IConfiguration superiorPlatformConfiguration = null, Action<JT808Configuration> options = null)
        {
            if (superiorPlatformConfiguration != null)
            {
                builder.Services.Configure<JT808Configuration>(superiorPlatformConfiguration.GetSection("JT808Configuration"));
            }
            if (options != null)
            {
                builder.Services.Configure(options);
            }

            //上级平台,服务端会话管理
            builder.Services.TryAddSingleton<JT808SessionManager>();
            return builder;
        }
    }
}