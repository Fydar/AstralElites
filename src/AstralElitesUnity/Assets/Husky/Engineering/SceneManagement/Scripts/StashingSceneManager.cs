using HuskyUnity.Engineering.SceneManagement.BootFlow;
using HuskyUnity.Engineering.SceneOwnership;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace HuskyUnity.Engineering.SceneManagement
{
	/// <summary>
	/// <para>A <see cref="StashingSceneManager"/> will stash away the entry scene and boot from the <c>Root</c> scene.</para>
	/// <para>Stashed scenes will have all of their root <see cref="GameObject"/>s disabled until the scene is opened again.</para>
	/// </summary>
	public class StashingSceneManager : ISceneManager
	{
		private class SceneStash
		{
			public Scene scene;
			public GameObject[] rootGameObjects;
			public bool[] defaultActivities;
			public bool[] defaultIsHiddenInHierarchy;
		}

		private readonly ISceneOwner sceneOwner;
        private readonly IBootFlow bootFlow;
        private readonly List<SceneStash> stashes;
		private readonly List<Scene> scenesToStashWhenLoaded;

		/// <summary>
		/// Creates a new instance of the <see cref="StashingSceneManager"/> with an <see cref="ISceneOwner"/> to assign to all loaded scenes.
		/// </summary>
		/// <param name="sceneOwner">The <see cref="ISceneOwner"/> to assign to all loaded scenes.</param>
		public StashingSceneManager(ISceneOwner sceneOwner, IBootFlow bootFlow)
		{
			this.sceneOwner = sceneOwner;
            this.bootFlow = bootFlow;
            stashes = new List<SceneStash>();
			scenesToStashWhenLoaded = new List<Scene>();

			SceneManager.sceneLoaded += OnAfterSceneLoadedCallback;
			SceneManager.sceneUnloaded += TryForgetStashForScene;
		}

		/// <summary>
		/// Disables all active scenes which <b>don't</b> have an <see cref="ISceneOwner"/> associated with them and launches
		/// the boot flow on the boot scene.
		/// </summary>
		public void StartBootFlow()
		{
			// All currently active scenes at the time of activation all called "Entry Scenes".
			int sceneCount = SceneManager.sceneCount;
			var entryScenes = new List<Scene>(sceneCount);
			Scene? bootScene = null;
			for (int i = 0; i < sceneCount; i++)
			{
				var scene = SceneManager.GetSceneAt(i);

				if (!OwnershipUtilities.IsOwned(scene))
				{
					OwnershipUtilities.ClaimOwnership(sceneOwner, scene);

					if (bootScene == null && scene.buildIndex == 0)
					{
						bootScene = scene;
					}
					else
					{
						StashScene(scene);
					}
				}
				entryScenes.Add(scene);
			}

			// If the entry scenes doesn't contain a boot scene, load one.
			if (bootScene == null)
			{
				var operation = SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
				bootScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
				OwnershipUtilities.ClaimOwnership(sceneOwner, bootScene.Value);
			}

			// Wait until the boot scene is loaded and run the boot flow.
			if (bootScene.Value.isLoaded)
			{
				SceneManager.SetActiveScene(bootScene.Value);
#if UNITY_EDITOR
				EditorSceneManager.MoveSceneBefore(bootScene.Value, SceneManager.GetSceneAt(0));
#endif
				RunBootFlow(bootScene.Value);
			}
			else
			{
				SceneManager.sceneLoaded += BootSceneCheckCallback;
				void BootSceneCheckCallback(Scene scene, LoadSceneMode loadSceneMode)
				{
					if (scene != bootScene)
					{
						return;
					}

					SceneManager.sceneLoaded -= BootSceneCheckCallback;
					SceneManager.SetActiveScene(bootScene.Value);
#if UNITY_EDITOR
					EditorSceneManager.MoveSceneBefore(bootScene.Value, SceneManager.GetSceneAt(0));
#endif
					RunBootFlow(bootScene.Value);
				}
			}
		}

		/// <inheritdoc/>
		public LoadSceneAsyncOperation LoadSceneAsync(int sceneBuildIndex)
		{
			foreach (var stash in stashes)
			{
				if (stash.scene.buildIndex == sceneBuildIndex)
				{
					UnstashScene(stash);
					return LoadSceneAsyncOperation.FromUnstash();
				}
			}

			var operation = SceneManager.LoadSceneAsync(sceneBuildIndex, LoadSceneMode.Additive);

			var sceneToLoad = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
			OwnershipUtilities.ClaimOwnership(sceneOwner, sceneToLoad);

			return LoadSceneAsyncOperation.FromSceneLoad(operation);
		}

		/// <inheritdoc/>
		public LoadSceneAsyncOperation LoadSceneAsync(string sceneName)
		{
			foreach (var stash in stashes)
			{
				if (stash.scene.name == sceneName)
				{
					UnstashScene(stash);
					return LoadSceneAsyncOperation.FromUnstash();
				}
			}

			var operation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			var sceneToLoad = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
			OwnershipUtilities.ClaimOwnership(sceneOwner, sceneToLoad);

			return LoadSceneAsyncOperation.FromSceneLoad(operation);
		}

		/// <inheritdoc/>
		public AsyncOperation UnloadSceneAsync(Scene scene)
		{
			TryForgetStashForScene(scene);
			return SceneManager.UnloadSceneAsync(scene);
		}

		private void StashScene(Scene scene)
		{
			if (IsStashed(scene))
			{
				string sceneDisplayName = !string.IsNullOrEmpty(scene.name)
					? scene.name
					: "Untitled";

				throw new InvalidOperationException($"Cannot stash scene \"{sceneDisplayName}\" as it is already stashed.");
			}

			if (!scene.isLoaded)
			{
				scenesToStashWhenLoaded.Add(scene);
			}
			else
			{
#if UNITY_EDITOR
				var lastScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
				EditorSceneManager.MoveSceneAfter(scene, lastScene);
#endif

				var rootObjects = scene.GetRootGameObjects();

				bool[] defaultActivities = new bool[rootObjects.Length];
				bool[] defaultHideFlags = new bool[rootObjects.Length];
				for (int i = 0; i < rootObjects.Length; i++)
				{
					var rootObject = rootObjects[i];

					defaultActivities[i] = rootObject.activeSelf;
					rootObject.SetActive(false);

					defaultHideFlags[i] = rootObject.hideFlags.HasFlag(HideFlags.HideInHierarchy);
					rootObject.hideFlags |= HideFlags.HideInHierarchy;
				}

				stashes.Add(new SceneStash()
				{
					scene = scene,
					rootGameObjects = rootObjects,
					defaultActivities = defaultActivities,
					defaultIsHiddenInHierarchy = defaultHideFlags
				});
			}
		}

		private void UnstashScene(SceneStash stash)
		{
			for (int i = 0; i < stash.rootGameObjects.Length; i++)
			{
				var sceneObject = stash.rootGameObjects[i];

				sceneObject.SetActive(stash.defaultActivities[i]);

				if (!stash.defaultIsHiddenInHierarchy[i])
				{
					sceneObject.hideFlags &= ~HideFlags.HideInHierarchy;
				}
			}

			stashes.Remove(stash);
		}

		private void OnAfterSceneLoadedCallback(Scene scene, LoadSceneMode loadSceneMode)
		{
			// If we have enqueues this scene to be stashed upon it being loaded, do so.
			if (scenesToStashWhenLoaded.Contains(scene))
			{
				scenesToStashWhenLoaded.Remove(scene);
				StashScene(scene);
			}
		}

		private void TryForgetStashForScene(Scene scene)
		{
			for (int i = stashes.Count - 1; i >= 0; i--)
			{
				var stash = stashes[i];
				if (stash.scene == scene)
				{
					stashes.RemoveAt(i);
				}
			}
		}

		private bool IsStashed(Scene scene)
		{
			foreach (var stash in stashes)
			{
				if (stash.scene == scene)
				{
					return true;
				}
			}
			return false;
		}

		private void RunBootFlow(Scene bootScene)
		{
			bootFlow.Execute(bootScene);
		}
	}
}
