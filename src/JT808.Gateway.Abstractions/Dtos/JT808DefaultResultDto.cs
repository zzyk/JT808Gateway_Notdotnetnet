﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions.Dtos
{
    /// <summary>
    /// JT808 默认响应结果
    /// </summary>
    public class JT808DefaultResultDto: JT808ResultDto<string>
    {
        public JT808DefaultResultDto()
        {
            Data = "Hello,JT808 WebAPI";
            Code = JT808ResultCode.Ok;
        }
    }
}
