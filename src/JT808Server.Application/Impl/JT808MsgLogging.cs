using JT808.Gateway.Abstractions;
using JT808.Gateway.Abstractions.Enums;
using JT808.Protocol.Extensions;
using Microsoft.Extensions.Logging;

namespace JT808Server.Application.Impl
{
    /// <summary>
    /// JT808上下行消息日志
    /// </summary>
    public class JT808MsgLogging : IJT808MsgLogging
    {
        private readonly ILogger Logger;
        public JT808MsgLogging(ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<JT808MsgLogging>();
        }
        public void Processor((string TerminalNo, byte[] Data) parameter, JT808MsgLoggingType jT808MsgLoggingType)
        {
            if(Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug($"{jT808MsgLoggingType.ToString()}-{parameter.TerminalNo}-{parameter.Data.ToHexString()}");
            }
        }
    }
}
