using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HuskyUnity.Engineering.Bootstrap.Internal
{
    internal class DefaultHost : IHost
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IHostedService[] hostedServices;

        public IExecutionContext ExecutionContext { get; }

        public IReadOnlyList<IHostedService> HostedServices => hostedServices;

        internal DefaultHost(
            IExecutionContext executionContext,
            IHostedService[] hostedServices)
        {
            ExecutionContext = executionContext;
            this.hostedServices = hostedServices;
        }

        public void StartHost()
        {
            foreach (var hostedService in hostedServices)
            {
                hostedService.OnStartHost(this);
            }
        }

        public int StopHost()
        {
            foreach (var hostedService in hostedServices)
            {
                hostedService.OnStopHost(this);
            }
            foreach (var hostedService in hostedServices)
            {
                if (hostedService is IDisposable startupDisposable)
                {
                    startupDisposable.Dispose();
                }
            }
            return 0;
        }
    }
}
