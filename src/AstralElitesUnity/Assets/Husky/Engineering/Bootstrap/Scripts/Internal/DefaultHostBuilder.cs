using System.Collections.Generic;

namespace HuskyUnity.Engineering.Bootstrap.Internal
{
    internal class DefaultHostBuilder : IHostBuilder
    {
        private readonly List<IHostedService> hostedServices = new();

        public IExecutionContext ExecutionContext { get; }

        internal DefaultHostBuilder(
            IExecutionContext executionContext)
        {
            ExecutionContext = executionContext;
        }

        public IHost Build()
        {
            return new DefaultHost(ExecutionContext, hostedServices.ToArray());
        }

        public IHostBuilder UseHostedService<TStartup>()
            where TStartup : IHostedService, new()
        {
            hostedServices.Add(new TStartup());
            return this;
        }

        public IHostBuilder UseHostedService<THostedService>(THostedService hostedService)
            where THostedService : IHostedService
        {
            hostedServices.Add(hostedService);
            return this;
        }
    }
}
