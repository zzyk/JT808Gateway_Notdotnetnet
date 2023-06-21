using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JT808Server.Utility.Common
{
    public class StringHelper
    {
        /// <summary>
        /// 根据EndPoint获取IPAddress
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static string GetIPAddress(EndPoint endPoint)
        {
            if (endPoint != null)
            {
                IPEndPoint iPEndPoint = endPoint as IPEndPoint;
                var ip = iPEndPoint.Address.ToString().Replace("::ffff:", "");
                var port = iPEndPoint.Port;
                return ip + ":" + port;
            }
            return string.Empty;
        }

    }
}
