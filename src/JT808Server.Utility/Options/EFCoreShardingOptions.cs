using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility.Options
{
    /// <summary>
    /// EfCore分表配置
    /// </summary>
    public class EFCoreShardingOptions
    {
        /// <summary>
        /// 分表和后缀连接字符
        /// </summary>
        public string TableSeparator { get; set; }
        /// <summary>
        /// 定时创建Gps分表Cron
        /// </summary>
        public string[] AutoCreateGpsShardTableCron { get; set; }
    }
}
