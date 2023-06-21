namespace JT808Server.Application.Contracts.Constant
{
    public static class WinFromDisplay
    {
        /// <summary>
        /// 显示日志信息事件 winfrom专用
        /// </summary>
        public static Action<string,string> ShowSysMsg { get; set; }
        /// <summary>
        /// 显示链路交互信息事件 winfrom专用
        /// </summary>
        public static Action<string, string, string, string, string> ShowOperatorLineMsg { get; set; }
        /// <summary>
        /// 显示跟踪信息事件 并记录日志文件 winfrom专用
        /// </summary>
        public static Action<string, string, int, string, string, string> ShowOperatorTrackMsg { get; set; }
        /// <summary>
        /// 移除登录信息事件 winfrom专用
        /// </summary>
        public static Action<dynamic> ClientLoginRemove { get; set; }

        /// <summary>
        /// 跟踪的车牌号 winfrom专用
        /// </summary>
        public static string TrackVehNo { get; set; }

    }
}
