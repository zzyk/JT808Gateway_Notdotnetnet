using JT808.Protocol;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// JT808网关构造器
    /// </summary>
    public interface IJT808GatewayBuilder
    {
        /// <summary>
        /// JT808构造器
        /// </summary>
        IJT808Builder JT808Builder { get; }
        IJT808Builder Builder();
    }
}
