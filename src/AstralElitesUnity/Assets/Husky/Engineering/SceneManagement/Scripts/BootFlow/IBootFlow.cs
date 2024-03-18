using UnityEngine.SceneManagement;

namespace HuskyUnity.Engineering.SceneManagement.BootFlow
{
	public interface IBootFlow
	{
		public void Execute(
			Scene bootScene);
	}
}
