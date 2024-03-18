namespace HuskyUnity.Engineering.Bootstrap
{
	/// <summary>
	/// <para>The execution context that is used to launch the game.</para>
	/// </summary>
	public interface IExecutionContext
	{
		/// <summary>
		/// <para>Command line arguments past into the application.</para>
		/// </summary>
		/// <remarks>
		/// <para>Inside the Unity Editor, these can be mocked with launch settings.</para>
		/// </remarks>
		public string[] CommandLineArguments { get; }
	}
}
