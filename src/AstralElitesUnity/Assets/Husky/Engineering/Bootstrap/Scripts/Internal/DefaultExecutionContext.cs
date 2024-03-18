namespace HuskyUnity.Engineering.Bootstrap.Internal
{
	internal class DefaultExecutionContext : IExecutionContext
	{
		public string[] CommandLineArguments { get; }

		internal DefaultExecutionContext(
			string[] commandLineArguments)
		{
			CommandLineArguments = commandLineArguments;
		}
	}
}
