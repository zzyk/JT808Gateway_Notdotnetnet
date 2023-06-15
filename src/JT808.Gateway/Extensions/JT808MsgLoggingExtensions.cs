﻿using JT808.Gateway.Abstractions;
using JT808.Gateway.HostedServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Extensions
{
    /// <summary>
    /// JT808 上下行消息日志扩展
    /// </summary>
    public static class JT808MsgLoggingExtensions
    {
        /// <summary>
        /// 添加上下行消息日志
        /// </summary>
        /// <typeparam name="TJT808MsgLogging"></typeparam>
        /// <param name="jT808ClientBuilder"></param>
        /// <returns></returns>
        public static IJT808ClientBuilder AddMsgLogging<TJT808MsgLogging>(this IJT808ClientBuilder jT808ClientBuilder)
            where TJT808MsgLogging: IJT808MsgLogging
        {
            jT808ClientBuilder.JT808Builder.Services.AddSingleton(typeof(IJT808MsgLogging),typeof(TJT808MsgLogging));
            jT808ClientBuilder.JT808Builder.Services.AddHostedService<JT808MsgDownLoggingHostedService>();
            jT808ClientBuilder.JT808Builder.Services.AddHostedService<JT808MsgDownReplyLoggingHostedService>();
            jT808ClientBuilder.JT808Builder.Services.AddHostedService<JT808MsgUpLoggingHostedService>();
            return jT808ClientBuilder;
        }

        public static IJT808GatewayBuilder AddMsgLogging<TJT808MsgLogging>(this IJT808GatewayBuilder jT808GatewayBuilder)
            where TJT808MsgLogging : IJT808MsgLogging
        {
            jT808GatewayBuilder.JT808Builder.Services.AddSingleton(typeof(IJT808MsgLogging), typeof(TJT808MsgLogging));
            return jT808GatewayBuilder;
        }
    }
}
