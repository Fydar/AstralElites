using UnityEngine;

public class Game
{

}

public class GameManager : MonoBehaviour
{
	public enum PlayState
	{
		Pregame,
		Playing,
		Paused,
		Ended
	}

	public static GameManager instance;

	public bool paused = false;
	public PlayState playState;
	public Game game;

	[Header ("Values")]
	public GlobalInt Score;
	public GlobalInt Highscore;
	public GlobalInt AsteroidsDestroyed;
	public GlobalFloat DistanceTravelled;

	[Space]

	public float GameDuration = 0.0f;

	[Header ("Scene")]
	public AsteroidGenerator generator;
	public Player player;

	[Space]

	public GameObject HUD;
	public GameObject End;
	public GameObject Community;

	private AnalyticsManager analytic;

	public SfxGroup OpenPauseMenu;
	public SfxGroup ClosePauseMenu;

	public CanvasGroup Fader;
	public float fadeSpeed = 2.5f;

	private IInterpolator Interpolator;

	private bool hasStarted = false;

	private void Awake ()
	{
		instance = this;
		analytic = GetComponent<AnalyticsManager> ();

		Interpolator = new LinearInterpolator () { Speed = fadeSpeed };
		instance = this;
	}

	private void Start ()
	{
		AudioManager.PlayMusic ("Main");
		Score.Value = 0;

		StartGame ();

		player.Health.OnAfterChanged += () =>
		{
			if (player.Health.Value <= 0.0f)
			{
				if (player.isAlive)
				{
					player.Kill ();
					AudioManager.Play (player.DestroySound);
					ScreenEffect.instance.Pulse (1.0f);
					Camera.main.GetComponent<PerlinShake> ().PlayShake (1.0f);
					EndGame ();
				}
			}
			else if (!player.isAlive)
			{
				player.Revive ();

				HUD.SetActive (true);
				if (Community != null)
				{
					Community.SetActive (false);
				}

				End.SetActive (false);
			}
		};
		hasStarted = true;
	}

	private void OnApplicationFocus (bool hasFocus)
	{
		if (hasStarted)
		{
			Pause ();
		}
	}

	private void Update ()
	{
		if (playState == PlayState.Playing)
		{
			if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.Menu))
			{
				UI_TogglePause ();
			}

			GameDuration += Time.deltaTime;
		}

		Interpolator.Update (Time.unscaledDeltaTime);
		Fader.alpha = Interpolator.Value;
		Fader.gameObject.SetActive (Interpolator.Value > 0.05f);

#if UNITY_EDITOR
		if (Input.GetKeyDown (KeyCode.X))
		{
			Score.Value += 1000;
		}
#endif
	}

	public void UI_TogglePause ()
	{
		if (paused)
		{
			UI_Unpause ();
		}
		else
		{
			UI_Pause ();
		}
	}

	public void UI_Pause ()
	{
		Pause ();
		AudioManager.Play (OpenPauseMenu);
	}

	public void UI_Unpause ()
	{
		if (!paused)
		{
			return;
		}

		Time.timeScale = 1.0f;
		paused = false;
		if (Community != null)
		{
			Community.SetActive (false);
		}

		Interpolator.TargetValue = 0.0f;

		AudioManager.Play (ClosePauseMenu);
	}

	public void UI_Exit ()
	{
		Application.Quit ();
	}

	private void Pause ()
	{
		if (playState != PlayState.Playing)
		{
			return;
		}

		if (paused)
		{
			return;
		}

		Debug.Log ("Pausing");
		Time.timeScale = 0.0f;
		paused = true;

		if (Community != null)
		{
			Community.SetActive (true);
		}

		Interpolator.TargetValue = 1.0f;
	}


	public void StartGame ()
	{
		player.Health.Value = 100;
		Score.Value = 0;
		AsteroidsDestroyed.Value = 0;
		DistanceTravelled.Value = 0;

		player.GetComponent<Rigidbody2D> ().velocity = Vector2.zero;

		AsteroidGenerator.instance.AsteroidPool.Flush ();
		generator.Enable ();

		DiscordController.Instance.StartNewGame ();

		analytic.Clear ();
		analytic.StartCapture ();
		GameDuration = 0.0f;

		playState = PlayState.Playing;
	}

	public void EndGame ()
	{
		generator.Disable ();

		HUD.SetActive (false);
		End.SetActive (true);
		if (Community != null)
		{
			Community.SetActive (true);
		}

		DiscordController.Instance.EndGame (Score.Value);
		analytic.EndCapture ();

		playState = PlayState.Ended;
	}

	public static void ScorePoints (int score)
	{
		instance.AsteroidsDestroyed.Value += 1;
		instance.Score.Value += score;

		if (instance.Highscore.Value < instance.Score.Value)
		{
			instance.Highscore.Value = instance.Score.Value;
		}
	}
}