using System;
using UnityEngine;

namespace HuskyUnity.Engineering.SceneManagement
{
	public class LoadSceneAsyncOperation
	{
		private readonly AsyncOperation sceneLoadOperation;

		public bool IsComplete { get; private set; }

		public float Progress
		{
			get
			{
				if (sceneLoadOperation != null)
				{
					return sceneLoadOperation.progress;
				}

				return 1.0f;
			}
		}

		public event Action OnCompleted;

		private LoadSceneAsyncOperation(
			AsyncOperation sceneLoadOperation)
		{
			this.sceneLoadOperation = sceneLoadOperation;

			if (sceneLoadOperation == null)
			{
				IsComplete = true;
			}
			else if (sceneLoadOperation.isDone)
			{
				IsComplete = true;
			}
			else
			{
				sceneLoadOperation.completed += SceneLoadOperation_completed;
			}
		}

		private void SceneLoadOperation_completed(
			AsyncOperation obj)
		{
			OnCompleted?.Invoke();
			IsComplete = true;
		}

		public static LoadSceneAsyncOperation FromSceneLoad(
			AsyncOperation asyncOperation)
		{
			return new LoadSceneAsyncOperation(asyncOperation);
		}

		public static LoadSceneAsyncOperation FromUnstash()
		{
			return new LoadSceneAsyncOperation(null)
			{
				IsComplete = true,
			};
		}
	}
}
