using UnityEngine;

namespace HuskyUnity.Engineering.Bootstrap
{
	/// <summary>
	/// <para>A base class for all "Programs" which are used to configure <see cref="IHostBuilder"/>.</para>
	/// </summary>
	public abstract class Program : ScriptableObject
	{
		/// <summary>
		/// <para>Configures the <see cref="IHostBuilder"/> with behaviours defined by this <see cref="Program"/>.</para>
		/// </summary>
		/// <param name="hostBuilder">The <see cref="IHostBuilder"/> to configure.</param>
		public abstract void ConfigureHostBuilder(
			IHostBuilder hostBuilder);
	}
}
