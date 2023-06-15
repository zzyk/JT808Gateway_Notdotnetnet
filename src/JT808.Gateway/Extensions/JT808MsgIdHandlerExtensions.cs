using JT808.Gateway.Abstractions;
using JT808.Gateway.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Extensions
{
    /// <summary>
    /// JT808 消息业务处理扩展
    /// </summary>
    public static class JT808MsgIdHandlerExtensions
    {
        /// <summary>
        /// 添加消息业务处理
        /// </summary>
        /// <typeparam name="TJT808UpMessageHandler"></typeparam>
        /// <param name="jT808ClientBuilder"></param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddMsgIdHandler<TJT808UpMessageHandler>(this IJT808ClientBuilder jT808ClientBuilder)
            where TJT808UpMessageHandler : IJT808UpMessageHandler
        {
            jT808ClientBuilder.JT808Builder.Services.AddSingleton(typeof(IJT808UpMessageHandler),typeof(TJT808UpMessageHandler));
            jT808ClientBuilder.JT808Builder.Services.AddHostedService<JT808MsgIdHandlerHostedService>();
            return jT808ClientBuilder;
        }
    }
}
