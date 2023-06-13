using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions.Configurations
{
    /// <summary>
    /// 远程服务选项
    /// </summary>
    public class RemoteServerOptions
    {
        public List<DataTransferOptions>  DataTransfer { get; set; }
    }
}
