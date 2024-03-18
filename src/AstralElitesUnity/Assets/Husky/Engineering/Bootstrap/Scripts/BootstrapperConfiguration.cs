using System.Diagnostics;
using UnityEngine;

namespace HuskyUnity.Engineering.Bootstrap
{
	/// <summary>
	/// <para>Configuration for the Bootstrapper.</para>
	/// </summary>
	public class BootstrapperConfiguration : ScriptableObject
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never), SerializeField] private Program program;

		/// <summary>
		/// <para>The <see cref="Bootstrap.Program"/> used to configure the primary <see cref="IHost"/> of the application.</para>
		/// </summary>
		public Program Program => program;
	}
}
