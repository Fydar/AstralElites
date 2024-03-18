using HuskyUnity.Engineering.Bootstrap;
using System.Diagnostics;
using UnityEngine;

namespace HuskyUnity.Gameplay.Anatomy
{
	/// <summary>
	/// <para>The <see cref="Program"/> used by the Husky project.</para>
	/// </summary>
	/// <remarks>
	/// <para>Configures the host with the <see cref="GameplayHostedService"/> hosted service.</para>
	/// </remarks>
	[CreateAssetMenu(menuName = "Husky/GameplayProgram")]
	public class GameplayProgram : Program
	{
		/// <inheritdoc/>
		public override void ConfigureHostBuilder(
			IHostBuilder hostBuilder)
		{
			hostBuilder.UseHostedService(new GameplayHostedService(this));
		}
	}
}
