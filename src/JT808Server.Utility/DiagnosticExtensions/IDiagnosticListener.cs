using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility
{
    public interface IDiagnosticListener : IObserver<KeyValuePair<string, object>>
    {
        string ListenerName { get; }
    }
}
