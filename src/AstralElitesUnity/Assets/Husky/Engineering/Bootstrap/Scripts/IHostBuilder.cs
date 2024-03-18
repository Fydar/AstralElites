namespace HuskyUnity.Engineering.Bootstrap
{
	/// <summary>
	/// <para>A builder for <see cref="IHost"/> instances.</para>
	/// </summary>
	public interface IHostBuilder
	{
		/// <summary>
		/// <para>Context about the execution of the host.</para>
		/// </summary>
		public IExecutionContext ExecutionContext { get; }

		/// <summary>
		/// <para>Creates a new <see cref="IHost"/> instance from the current state of this builder.</para>
		/// </summary>
		/// <returns>The newly constructed <see cref="IHost"/> from the current state of this builder.</returns>
		public IHost Build();

		/// <summary>
		/// <para>Registers an additional <see cref="IHostedService"/> to constructed <see cref="IHost"/> instances.</para>
		/// </summary>
		/// <typeparam name="THostedService">The type of <see cref="IHostedService"/> to register.</typeparam>
		/// <returns>The current instance of this builder for continued construction.</returns>
		public IHostBuilder UseHostedService<THostedService>()
			where THostedService : IHostedService, new();

		/// <summary>
		/// <para>Registers an additional <see cref="IHostedService"/> to constructed <see cref="IHost"/> instances.</para>
		/// </summary>
		/// <typeparam name="THostedService">The type of <see cref="IHostedService"/> to register.</typeparam>
		/// <param name="hostedService">The <see cref="IHostedService"/> instance to register.</param>
		/// <returns>The current instance of this builder for continued construction.</returns>
		public IHostBuilder UseHostedService<THostedService>(THostedService hostedService)
			where THostedService : IHostedService;
	}
}
