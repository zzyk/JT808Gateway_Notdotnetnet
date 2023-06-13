using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions.Configurations
{
    /// <summary>
    /// 数据转发选项
    /// </summary>
    public class DataTransferOptions
    {
        /// <summary>
        /// 主机
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// sim卡号
        /// </summary>
        public List<string> TerminalNos { get; set; }
    }
}
