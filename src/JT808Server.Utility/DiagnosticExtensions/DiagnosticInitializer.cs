using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT808Server.Utility
{
    public class DiagnosticInitializer : IObserver<DiagnosticListener>
    {

        private readonly IEnumerable<IDiagnosticListener> _diagnosticListeners;
        private readonly object _lock = new();
        public DiagnosticInitializer(IEnumerable<IDiagnosticListener> diagnosticListeners)
        {
            _diagnosticListeners = diagnosticListeners;
        }
        void IObserver<DiagnosticListener>.OnNext(DiagnosticListener value)
        {
            foreach (var listener in _diagnosticListeners)
            {
                if (listener.ListenerName == value.Name)
                {
                    lock (_lock)
                    {
                        value.Subscribe(listener!);
                    }
                }
            }
        }

        void IObserver<DiagnosticListener>.OnError(Exception error) { }
        void IObserver<DiagnosticListener>.OnCompleted() { }
    }
}
