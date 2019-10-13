using UnityEngine;
using UnityEngine.UI;

public class AnalyticsRenderer : MonoBehaviour
{
	public AnalyticsManager manager;

	[Header ("Rendering")]
	public Text LastCollisionDetails;

	[Space]

	public Text DurationText;
	public Text ScoreText;
	public Text RankText;
	[Space]
	public RankRender RankDisplay;
	[Space]
	public Text TotalAsteroidsText;
	public Text PerformanceText;
	public Text DistanceTravelledText;
	public Text AverageSpeedText;

	[Space]

	public Chart_Basic ScoreChartA;
	public Chart_Basic ScoreChartB;

	[Space]

	public Chart_Basic HealthChartA;
	public Chart_Basic HealthChartB;

	private GameManager gameManager;

	public void OnEnable ()
	{
		gameManager = GameManager.instance;
		if (gameManager == null)
		{
			return;
		}

		ScoreChartA.SetData (manager.scoreHook.data.ToArray ());
		ScoreChartB.SetData (manager.scoreHook.data.ToArray ());

		HealthChartA.SetData (manager.healthHook.data.ToArray ());
		HealthChartB.SetData (manager.healthHook.data.ToArray ());

		int seconds = ((int)gameManager.GameDuration) % 60;
		int minutes = (int)(gameManager.GameDuration / 60);

		DurationText.text = minutes.ToString () + "m " + seconds.ToString () + "s";
		ScoreText.text = gameManager.Score.Value.ToString ("###,##0");

		var rank = Rank.GetRank (gameManager.Score.Value);
		RankText.text = rank.DisplayName;

		if (RankDisplay != null)
		{
			RankDisplay.RenderRank (rank);
		}

		TotalAsteroidsText.text = gameManager.AsteroidsDestroyed.Value.ToString ();
		PerformanceText.text = (gameManager.AsteroidsDestroyed.Value / gameManager.GameDuration)
			.ToString ("0.00") + " Asteroids per Second";
		DistanceTravelledText.text = gameManager.DistanceTravelled.Value.ToString ("###,##0.00") + " m";
		AverageSpeedText.text = (gameManager.DistanceTravelled.Value / gameManager.GameDuration).ToString ("###,##0.00") + " m/s";

		if (LastCollisionDetails != null)
		{
			LastCollisionDetails.text = "Your where crused by an asteroid moving " +
				manager.Collisions.Last.relativeVelocity.magnitude.ToString ("###,##0.00") + " m/s.";
		}
	}
}