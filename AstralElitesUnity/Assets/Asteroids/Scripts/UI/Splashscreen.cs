using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Splashscreen : MonoBehaviour
{
	public enum State
	{
		Loading,
		Waiting,
		Loaded
	}

	[Header ("Animation")]
	public Animator ScreenAnimation;
	public AudioListener audioListener;

	[Header ("Parameters")]
	public PrefsBool DiscordTOS;
	public string SceneName = "Game";
	public string TosScene = "EULA";

	[Space]
	public State state;

	private IInterpolator ProgressInterpolator;
	private int awaitAnimationID;

	private void Awake ()
	{
		ProgressInterpolator = new LinearInterpolator () { Speed = 1.25f };

		awaitAnimationID = Animator.StringToHash ("Continue");
	}

	private void Start ()
	{
		StartCoroutine (ScreenFlow ());
	}

	private void Update ()
	{
		ProgressInterpolator.Update (Time.deltaTime);
	}

	private IEnumerator ScreenFlow ()
	{
		string sceneToLoad = SceneName;
		if (!DiscordTOS.Value)
		{
			sceneToLoad = TosScene;
		}

		state = State.Loading;
		var async = SceneManager.LoadSceneAsync (sceneToLoad, LoadSceneMode.Additive);
		async.allowSceneActivation = false;

		while (async.progress != 0.9f)
		{
			ProgressInterpolator.TargetValue = async.progress;
			yield return null;
		}

		state = State.Waiting;

		while (true)
		{
			var animStateInfo = ScreenAnimation.GetCurrentAnimatorStateInfo (0);

			if (animStateInfo.shortNameHash == awaitAnimationID)
			{
				break;
			}

			yield return null;
		}

		audioListener.enabled = false;
		Destroy (audioListener);

		async.allowSceneActivation = true;
		state = State.Loaded;

		SceneManager.UnloadSceneAsync (gameObject.scene);

		gameObject.SetActive (false);
	}
}
