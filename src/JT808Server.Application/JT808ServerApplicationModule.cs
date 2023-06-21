using JT808.Gateway.Abstractions.Configurations;
using JT808.Protocol;
using JT808.Protocol.Enums;
using JT808.Gateway.Client;
using JT808.Gateway.WebApiClientTool;
using JT808.Gateway;
using JT808.Gateway.Extensions;
using JT808Server.Application.Automappper;
using JT808Server.Application.Contracts;
using JT808Server.Application.Customs;
using JT808Server.Application.Impl;
using JT808Server.Domain;
using JT808Server.Utility.Common;
using JT808Server.Utility.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Volo.Abp.Guids;
using Volo.Abp.Modularity;
using ProtoBuf.Meta;
using JT808Server.Application.Jobs;

namespace JT808Server.Application
{
    [DependsOn(
        typeof(AbpGuidsModule),
        typeof(JT808ServerDomainModule),
        typeof(JT808ServerApplicationContractsModule))]
    public class JT808ServerApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            var configuration = context.Services.GetConfiguration();
            context.Services.Configure<JT808Configuration>(configuration.GetSection("JT808Configuration"));
            context.Services.Configure<SystemConfiguration>(configuration.GetSection("SystemConfiguration"));
            var section = configuration.GetSection("JT808Configuration");
            //根据配置加载对应的标准
            JT808Configuration _jT808Configuration = section.Get<JT808Configuration>();

            //注册808
            IJT808Builder _jt808Builder = context.Services.AddJT808Configure();

            //添加客户端工具
            _jt808Builder.AddJT808Core().AddClient().Builder();

            //方式1:客户端webapi调用
            _jt808Builder.AddWebApiClientTool<JT808HttpClientExt>(context.Services.GetConfiguration())
                        .AddGateway(context.Services.GetConfiguration())
                        .AddMessageHandler<JT808CustomMessageHandlerImpl>()
                        .AddMsgReplyConsumer<JT808MsgReplyConsumer>()
                        .AddMsgLogging<JT808MsgLogging>()
                        .AddSessionNotice()
                        .AddTransmit(context.Services.GetConfiguration());
                        //.AddTcp()
                        //.AddUdp();
            //context.Services.AddHostedService<CallHttpClientJob>();
            //context.Services.AddSingleton<CallHttpClientJob>();
            ////客户端测试  依赖AddClient()服务
            //context.Services.AddHostedService<UpJob>();
            //context.Services.AddSingleton<UpJob>();
            //context.Services.AddSingleton<JT808TcpServer>();
            //context.Services.AddSingleton<JT808UdpServer>();
            //需要跨域的
            context.Services.AddCors(options =>
               options.AddPolicy("jt808", builder =>
               builder.AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials()
                      .SetIsOriginAllowed(o => true)));
            //添加AutoMapper映射
            context.Services.AddAutoMapper(typeof(AutoMapperConfig));
            //添加VehicleAnBiaoHttpClient(使用自定义类执行HttpClientFactory 请求)
            context.Services.AddHttpClient<VehicleAnBiaoHttpClient>(httpClient =>
            {
                string sUAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 Edg/96.0.1054.62";
                httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
                httpClient.DefaultRequestHeaders.Add("User-Agent", sUAgent);
                httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
                httpClient.BaseAddress = new Uri(configuration.GetSection("SystemConfiguration:AnBiaoUrl").Value);
            }).ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                //如果服务器有 https 证书，但是证书不安全，则需要使用下面语句  => 也就是说，不校验证书，直接允许
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, error) => true;
                //handler.ClientCertificates.Add(new X509Certificate("CerPath", "CerPassword"));
                handler.AllowAutoRedirect = true;
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                return handler;
            });
        }
    }
}
