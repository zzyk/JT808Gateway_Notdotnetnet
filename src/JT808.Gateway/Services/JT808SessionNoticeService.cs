using Microsoft.Extensions.Logging;

namespace JT808.Gateway.Services
{
    /// <summary>
    /// 会话通知（在线/离线）服务
    /// </summary>
    public class JT808SessionNoticeService
    {
        protected ILogger logger { get; }
        public JT808SessionNoticeService(ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<JT808SessionNoticeService>();
        }
        /// <summary>
        /// 处理
        /// </summary>
        /// <param name="parameter"></param>
        public virtual void Processor((string Notice, string TerminalNo) parameter)
        {
            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug($"{parameter.Notice}-{parameter.TerminalNo}");
            }
        }
    }
}
