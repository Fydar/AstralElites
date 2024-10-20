using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuskyUnity.Engineering.SceneManagement
{
    public interface ISceneManager
    {
        public LoadSceneAsyncOperation LoadSceneAsync(
            int sceneBuildIndex);

        public LoadSceneAsyncOperation LoadSceneAsync(
            string sceneName);

        public AsyncOperation UnloadSceneAsync(
            Scene scene);
    }
}
