using HuskyUnity.Engineering.Bootstrap;
using HuskyUnity.Engineering.SceneManagement;
using HuskyUnity.Engineering.SceneManagement.BootFlow;
using HuskyUnity.Engineering.SceneOwnership;
using System.Linq;
using UnityEngine.SceneManagement;

namespace HuskyUnity.Gameplay.Anatomy
{
	/// <summary>
	/// <para>The <see cref="IHostedService"/> implementation used for the Husky project.</para>
	/// </summary>
	public class GameplayHostedService : IHostedService, ISceneOwner
	{
		private readonly GameplayProgram gameplayProgram;

		internal GameplayHostedService(
			GameplayProgram gameplayProgram)
		{
			this.gameplayProgram = gameplayProgram;
		}

		/// <inheritdoc/>
		public void OnStartHost(IHost host)
		{
			var bootFlow = new BootFlow(host);
            var sceneManager = new StashingSceneManager(this, bootFlow);
			bootFlow.SceneManager = sceneManager;

			sceneManager.StartBootFlow();
        }

		/// <inheritdoc/>
		public void OnStopHost(IHost host)
		{
		}
	}

    public class BootFlow : IBootFlow
    {
		public StashingSceneManager SceneManager { get; internal set; }

		private IHost host;

        internal BootFlow(
            IHost host)
        {
            this.host = host;
        }

        public void Execute(Scene bootScene)
        {
			if (!host.ExecutionContext.CommandLineArguments.Contains("background"))
			{
				foreach (var rootObject in bootScene.GetRootGameObjects())
				{
					if (rootObject.name == "Managers")
					{
						var asteroidGenerator = rootObject.GetComponentInChildren<AsteroidGenerator>();
                        asteroidGenerator.Enable();
                    }
				}
			}
			else
            {
                SceneManager.LoadSceneAsync("Interface");
            }
        }
    }
}
