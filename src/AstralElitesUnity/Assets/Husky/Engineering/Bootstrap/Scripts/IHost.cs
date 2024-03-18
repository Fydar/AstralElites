using System.Collections.Generic;

namespace HuskyUnity.Engineering.Bootstrap
{
	/// <summary>
	/// A host that can be used to contain application components.
	/// </summary>
	public interface IHost
	{
		/// <summary>
		/// Launch settings used to configure this <see cref="IHost"/>.
		/// </summary>
		public IExecutionContext ExecutionContext { get; }

		/// <summary>
		/// All <see cref="IHostedService"/> instances associated with this <see cref="IHost"/>.
		/// </summary>
		public IReadOnlyList<IHostedService> HostedServices { get; }

		/// <summary>
		/// Starts the <see cref="IHost"/> by invoking all <see cref="IHostedService.OnStartHost(IHost)"/> callbacks.
		/// </summary>
		public void StartHost();

		/// <summary>
		/// Begins the host stopping by invoking all <see cref="IHostedService.OnStopHost(IHost)"/> callbacks.
		/// </summary>
		/// <returns>The status code the <see cref="IHost"/> closed with.</returns>
		public int StopHost();
	}
}
