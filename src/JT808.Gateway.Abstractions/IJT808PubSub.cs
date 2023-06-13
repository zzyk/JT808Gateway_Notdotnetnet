using System;
using System.Collections.Generic;
using System.Text;

namespace JT808.Gateway.Abstractions
{
    /// <summary>
    /// 发布订阅接口
    /// </summary>
    public interface IJT808PubSub
    {
        string TopicName { get; }
    }
}
